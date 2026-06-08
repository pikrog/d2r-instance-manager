using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Mappers;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;
using DynamicData.Kernel;

namespace AvaloniaApplication1.Services;

public class GameInstanceService(ConfigService configService, GameInstanceManager gameInstanceManager, AccountService accountService, RegionService regionService)
{
    public class GameInstanceValidationRules // todo: implement
    {
        
    }

    public class GameInstanceDraftValidator(AccountService accountService, RegionService regionService)
    {
        private readonly AccountService _accountService = accountService;
        private readonly RegionService _regionService = regionService;
        
        public void Validate(GameInstanceDraft draft)
        {
            if (draft.IsOnlineMode)
            {
                if (draft.AccountId is null)
                    throw new ArgumentException("Account id is required when using online mode");
            }
        }
    }
    
    private void Validate(GameInstanceDraft draft) // todo: move to validator
    {
        if (draft.IsOnlineMode)
        {
            if (draft.AccountId is null)
                throw new ArgumentException("Account id is required when using online mode");

            if (!accountService.Exists(draft.AccountId.Value))
                throw new ArgumentException(
                    $"Account with id {draft.AccountId} does not exist"); // todo: replace with a more specific exception

            if (draft.RegionId is null)
                throw new ArgumentException("Region id is required when using online mode");

            if (!regionService.Exists(draft.RegionId.Value))
                throw new ArgumentException($"Region with id {draft.RegionId} does not exist"); // todo: replace with a more specific exception
        }
        
        // todo: name uniqueness check -> in ConfigLoader
        if (string.IsNullOrWhiteSpace(draft.Name))
            throw new ArgumentException("Name is required");
    }

    private async Task Add(GameInstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.AddInstance(snapshot));
        gameInstanceManager.Register(snapshot.Id);
    }

    private async Task Update(GameInstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.UpdateInstance(snapshot));
    }

    public async Task Save(GameInstanceDraft draft)
    {
        Validate(draft);

        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new GameInstanceSnapshot(
            id,
            draft.Name,
            draft.IsOnlineMode,
            draft.AccountId,
            draft.CredentialsVector,
            draft.RegionId,
            draft.DisplayId,
            draft.IsNoSound,
            draft.IsWindowedMode,
            draft.RecallHotKey
        );

        if (draft.Id is null)
            await Add(snapshot);
        else
            await Update(snapshot);
    }
    
    public async Task Remove(Guid id)
    {
        await configService.ChangeAsync(context => context.RemoveInstance(id));
        gameInstanceManager.Remove(id);
    }

    public GameInstanceSnapshot GetInstanceSnapshot(Guid id)
    {
        return configService.Config.GetInstance(id);
    }

    public GlobalSettingsSnapshot GetSettingsSnapshot()
    {
        return configService.Config.GetGlobalSettings();
    }

    public List<GameInstanceTableRow> GetTable()
    {
        var instances = gameInstanceManager.GetAllRuntimeStates().ToDictionary(i => i.Id);
        return configService.Config.GetAllInstances().Select(i =>
            {
                instances.TryGetValue(i.Id, out var snapshot);
                var status = snapshot is not null  ? GameInstanceStatusMapper.Map(snapshot) : GameInstanceStatus.Unknown;
                return new GameInstanceTableRow(i.Id, i.Name, status);
            }).ToList();
    }

    public async Task Launch(Guid id)
    {
        var settings = GetSettingsSnapshot();
        
        var snapshot = GetInstanceSnapshot(id);
        AuthenticationContext authenticationContext = new OfflineAuthenticationContext();
        if (snapshot.IsOnlineMode)
        {
            var account = accountService.GetSnapshot(snapshot.AccountId!.Value);
            var region = regionService.GetSnapshot(snapshot.RegionId!.Value);
            
            authenticationContext = snapshot.CredentialsVector switch
            {
                CredentialsVector.CommandLineArguments => new CliAuthenticationContext(account.Username, account.Password,
                    region.Address),
                CredentialsVector.OsiTokenRegistry => new OsiAuthenticationContext(region.Address),
                _ => throw new InvalidOperationException($"Unknown credentials vector {snapshot.CredentialsVector}")
            };
        }
        
        var context = new LaunchContext(
            settings.GameExecutablePath,
            authenticationContext,
            snapshot.DisplayId,
            snapshot.IsNoSound,
            snapshot.IsWindowedMode
        );
        await gameInstanceManager.LaunchAsync(snapshot.Id, context);
    }
}

/*using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Services;

public class GameInstanceService
{
    private readonly SemaphoreSlim _instanceSemaphore = new(1, 1);

    private readonly object _entriesSync = new();

    private readonly object _idSync = new();

    public Settings Settings { get; set; }

    private int _nextEntryId = 1;
    private int NextEntryId
    {
        get
        {
            lock (_idSync)
            {
                return _nextEntryId++;
            }
        }
    }

    public record class PreviewEntry(int Id, Profile Profile, GameInstanceStatus InstanceStatus, bool InstanceIsStarted, bool IsRunning);

    private class Entry(int id, Profile profile, GameInstance instance)
    {
        public int Id { get; } = id;
        public Profile Profile { get; set; } = profile;
        public GameInstance Instance { get; set; } = instance;
        public bool IsRunning { get; set; }

        public PreviewEntry AsPreviewEntry() => new(Id, Profile.ShallowCopy(), Instance.Status, Instance.IsStarted, IsRunning);
    } 

    private readonly List<Entry> _entries = [];

    public IReadOnlyList<PreviewEntry> PreviewEntries
    {
        get
        {
            lock (_entriesSync)
            {
                return _entries.Select(e => e.AsPreviewEntry()).ToList().AsReadOnly();
            }
        }
    }

    public IReadOnlyList<Profile> Profiles
    {
        get
        {
            lock (_entriesSync)
            {
                return _entries.Select(e => e.Profile).ToList().AsReadOnly();
            }
        }
    }

    private List<GameInstance> Instances
    {
        get
        {
            lock (_entriesSync)
            {
                return [.. _entries.Select(e => e.Instance)];
            }
        }
    }

    private readonly Dictionary<int, Entry> _entryMap = [];

    // TODO: move to MainWindow (?)
    private readonly NativePlatform.GlobalHotKeyManager _hotKeyManager = new(false);

    public ILoggerRegistry LoggerRegistry;
        
    public ILogWriter Logger { get; }

    public class EntryChangedEventArgs(int entryId) : EventArgs
    {
        public int EntryId { get; set; } = entryId;
    }

    public event EventHandler<EntryChangedEventArgs>? EntryChanged;

    public event EventHandler<GameInstanceStatusChangedEventArgs>? InstanceStatusChanged;

    private readonly GameInstanceObserver _gameInstanceObserver = new();

    public GameInstanceService(Settings settings, IEnumerable<Profile> profiles, ILoggerRegistry loggerRegistry)
    {
        LoggerRegistry = loggerRegistry;
        Logger = LoggerRegistry.GetLogger<GameInstanceService>();

        Settings = settings;
        foreach (var profile in profiles)
            CoreAdd(profile);

        _gameInstanceObserver = new(LoggerRegistry.GetLogger<GameInstanceObserver>());
        InstanceStatusChanged += _gameInstanceObserver.InstanceStatusChanged;
    }

    private void InvokeEntryChangedEvent(int entryId)
    {
        var entryChangedEventArgs = new EntryChangedEventArgs(entryId);
        var entryChangedhandlers = EntryChanged;
        entryChangedhandlers?.Invoke(this, entryChangedEventArgs);
    }

    private void Internal_InstanceStatusChanged(object? sender, GameInstanceStatusChangedEventArgs statusChangedEventArgs)
    {
        var instance = (sender as GameInstance)!;

        if (!_entryMap.TryGetValue(instance.Id, out _))
        {
            Logger.Warning($"Instance <{instance.Name}> refers to an unknown Entry <{instance.Id}>");
            return;
        }

        InvokeEntryChangedEvent(instance.Id);

        var instanceStatusChangedHandlers = InstanceStatusChanged;
        instanceStatusChangedHandlers?.Invoke(sender, statusChangedEventArgs);
    }

    private int CoreAdd(Profile profile)
    {
        var entryId = NextEntryId;

        var instance = new GameInstance(entryId, profile.Name, _instanceSemaphore, new(), Settings);
        instance.StatusChanged += Internal_InstanceStatusChanged;

        var entry = new Entry(entryId, profile, instance);

        lock (_entriesSync)
        {
            _entries.Add(entry);
            _entryMap[entryId] = entry;
        }

        var eventArgs = new EntryChangedEventArgs(entryId);
        var handlers = EntryChanged;
        handlers?.Invoke(this, eventArgs);

        return entryId;
    }

    public int Add(Profile profile)
    {
        var result = CoreAdd(profile);

        Logger.Info($"Profile <{profile.Name}> added");

        return result;
    }

    private Entry? GetEntry(int entryId)
    {
        lock (_entriesSync)
        {
            return _entryMap.GetValueOrDefault(entryId);
        }
    }

    public PreviewEntry? GetPreviewEntry(int entryId)
    {
        lock (_entriesSync)
        {
            return GetEntry(entryId)?.AsPreviewEntry();
        }
    }

    public Profile? GetProfileByEntryId(int entryId)
    {
        lock ( _entriesSync)
        {
            return GetEntry(entryId)?.Profile.ShallowCopy();
        }
    }

    public enum ModifyResult
    {
        Success,
        EntryNotFound,
        InstanceIsRunning,
    }

    private ModifyResult CoreEdit(int entryId, Profile newProfile)
    {
        Entry? entry;
        lock (_entriesSync)
        {
            entry = GetEntry(entryId);
            if (entry is null)
                return ModifyResult.EntryNotFound;
            if (entry.IsRunning)
                return ModifyResult.InstanceIsRunning;
            entry.Profile = newProfile;
        }

        var eventArgs = new EntryChangedEventArgs(entryId);
        var handlers = EntryChanged;
        handlers?.Invoke(this, eventArgs);

        return ModifyResult.Success;
    }

    public ModifyResult Edit(int entryId, Profile newProfile)
    {
        var result = CoreEdit(entryId, newProfile);

        switch(result)
        {
            case ModifyResult.Success:
                Logger.Info($"Profile <{newProfile.Name}> saved at Entry <{entryId}>");
                break;
            case ModifyResult.EntryNotFound:
                Logger.Error($"Profile <{newProfile.Name}> not saved. Entry <{entryId}> not found");
                break;
            case ModifyResult.InstanceIsRunning:
                Logger.Error($"Profile <{newProfile.Name}> not saved. Entry <{entryId}> refers to a running instance");
                break;
        }   

        return result;
    }

    private ModifyResult CoreRemove(int entryId)
    {
        Entry? entry;
        lock (_entriesSync)
        {
            entry = GetEntry(entryId);
            if (entry is null)
                return ModifyResult.EntryNotFound;
            if (entry.IsRunning)
                return ModifyResult.InstanceIsRunning;
            entry.Instance.StatusChanged -= Internal_InstanceStatusChanged;
            _entries.Remove(entry);
            _entryMap.Remove(entryId);
        }
        return ModifyResult.Success;
    }

    public ModifyResult Remove(int entryId)
    {
        Logger.Info($"Requested to remove Entry <{entryId}>");

        var result = CoreRemove(entryId);

        switch(result)
        {
            case ModifyResult.Success:
                Logger.Info($"Entry <{entryId}> removed");
                break;
            case ModifyResult.EntryNotFound:
                Logger.Error($"Entry <{entryId}> not removed. Entry not found");
                break;
            case ModifyResult.InstanceIsRunning:
                Logger.Error($"Entry <{entryId}> not removed. Entry <{entryId}> refers to a running instance");
                break;
        }

        var eventArgs = new EntryChangedEventArgs(entryId);
        var handlers = EntryChanged;
        handlers?.Invoke(this, eventArgs);

        return result;
    }

    public bool IsGameExecutableValid()
    {
        return !string.IsNullOrEmpty(Settings.GameExecutablePath) && System.IO.File.Exists(Settings.GameExecutablePath);
    }

    private NativePlatform.Display? GetDisplay(Profile profile)
    {
        var display = NativePlatform.DisplayList.All.GetByIdentifier(profile.DisplayIdentifier);
        if (display is null)
        {
            Logger.Warning($"Profile <{profile.Name}> refers to an invalid Display Identifier <{profile.DisplayIdentifier}>");
            if (Settings.FallbackToPrimaryDisplayIfInvalid)
            {
                display = NativePlatform.DisplayList.All.FirstOrDefault();
                Logger.Warning($"Falling back to the primary display for Profile <{profile.Name}>");
            }
        }
        return display;
    }

    private Keys SetupRecallHotKey(Profile profile, GameInstance instance)
    {
        var recallHotKey = profile.HotKey;
        if (recallHotKey != Keys.None)
            _hotKeyManager.Set(recallHotKey, _ => Recall(instance.Id));
        return recallHotKey;
    }

    private void UnsetRecallHotKey(Keys hotKey)
    {
        if (hotKey != Keys.None)
            _hotKeyManager.Unset(hotKey);
    }

    public enum RunResult
    {
        Success,
        EntryNotFound,
        AlreadyRunning,
        InvalidIndex,
        InvalidExecutablePath,
        InvalidDisplay,
        HotKeyRegistrationFailed,
        UndefinedRegion,
        UnknownError,
    }

    public record class RunInfo(RunResult RunResult, Profile? Profile = null, Exception? Exception = null);

    private async Task<RunInfo> CoreRunAsync(int entryId, Action<GameInstance> instanceStarted)
    {
        var settings = Settings;
        Entry entry;
        Profile profile;
        lock (_entriesSync)
        {
            var _entry = GetEntry(entryId);
            if (_entry is null)
                return new RunInfo(RunResult.EntryNotFound);
            entry = _entry;
            profile = entry.Profile.ShallowCopy();

            if (entry.IsRunning)
                return new RunInfo(RunResult.AlreadyRunning, profile);
            entry.IsRunning = true;
        }

        try
        {
            InvokeEntryChangedEvent(entry.Id);

            if (!IsGameExecutableValid())
                return new RunInfo(RunResult.InvalidExecutablePath, profile);

            var display = GetDisplay(profile);
            if (display is null)
                return new RunInfo(RunResult.InvalidDisplay, profile);

            var argumentsFactory = new ProfileCommandLineArgumentListFactory(profile, settings);
            var argumentList = argumentsFactory.Create();
            var argumentString = NativePlatform.ManagedApplication.BuildArguments(argumentList);

            var app = new NativePlatform.ManagedApplication(
                settings.GameExecutablePath, 
                argumentString, 
                display,
                LoggerRegistry.GetLogger<NativePlatform.ManagedApplication>());
            try
            {
                GameInstance instance;
                lock (_entriesSync)
                {
                    instance = entry.Instance = entry.Instance.Continue(app, settings);
                }
                var recallHotKey = SetupRecallHotKey(profile, instance);
                try
                {
                    var task = instance.RunAsync().ConfigureAwait(false);
                    instanceStarted(instance);
                    await task;
                }
                finally
                {
                    UnsetRecallHotKey(recallHotKey);
                }
            }
            finally
            {
                app.Dispose();
            }
        }
        catch (RegionList.UndefinedRegionException e)
        {
            return new RunInfo(RunResult.UndefinedRegion, profile, e);
        }
        catch (NativePlatform.GlobalHotKeyManager.AlreadyRegisteredException e)
        {
            return new RunInfo(RunResult.HotKeyRegistrationFailed, profile, e);
        }
        catch (Exception e)
        {
            return new RunInfo(RunResult.UnknownError, profile, e);
        }
        finally
        {
            lock(_entriesSync)
            {
                entry.IsRunning = false;
            }
            InvokeEntryChangedEvent(entry.Id);
        }
        return new RunInfo(RunResult.Success, profile);
    }

    public async Task<RunInfo> RunAsync(int entryId)
    {
        void InstanceStarted(GameInstance instance) => Logger.Info($"Instance <{instance.Name}> started");

        var runInfo = await CoreRunAsync(entryId, InstanceStarted);

        string? errorDetails = null;
        switch (runInfo.RunResult)
        {
            case RunResult.EntryNotFound:
                errorDetails = $"Entry <{entryId}> not found";
                break;
            case RunResult.AlreadyRunning:
                errorDetails = "Instance is running already";
                break;
            case RunResult.InvalidExecutablePath:
                errorDetails = $"Invalid executable path";
                break;
            case RunResult.InvalidDisplay:
                errorDetails = $"Invalid Display Identifier <{runInfo.Profile!.DisplayIdentifier}>";
                break;
            case RunResult.UndefinedRegion:
            case RunResult.HotKeyRegistrationFailed:
                errorDetails = runInfo.Exception!.Message;
                break;
            case RunResult.UnknownError:
                errorDetails = "Unexpected error";
                break;

        }
        if(errorDetails is not null)
        {
            if (runInfo.Profile is null)
                Logger.Error($"Entry <{entryId}> instance run failed. {errorDetails}");
            else
                Logger.Error($"Instance <{runInfo.Profile.Name}> run failed. {errorDetails}");

            if(runInfo.RunResult == RunResult.UnknownError)
                Logger.Debug(runInfo.Exception!.ToString());
        }
        else
            Logger.Info($"Instance <{runInfo.Profile!.Name}> exited successfully");

        return runInfo;
    }

    public bool RequestStop(int entryId)
    {
        GameInstance instance;
        lock (_entriesSync)
        {
            var entry = GetEntry(entryId);
            if (entry is null)
                return false;
            instance = entry.Instance;
        }
        instance.RequestStop();
        return true;
    }

    public async Task StopAllAsync()
    {
        var stopTasks = Instances.Select(instance => instance.StopAsync());
        await Task.WhenAll(stopTasks).ConfigureAwait(false);
    }

    public enum RecallResult
    {
        Success,
        EntryNotFound,
        UnknownError,
    }

    public record class RecallInfo(RecallResult RecallResult, GameInstance? Instance = null, Exception? Exception = null);

    private RecallInfo CoreRecall(int entryId)
    {
        Profile profile; 
        GameInstance instance;
        lock (_entriesSync)
        {
            var entry = GetEntry(entryId);
            if (entry is null)
                return new RecallInfo(RecallResult.EntryNotFound);
            profile = entry.Profile;
            instance = entry.Instance;
        }

        try
        {
            instance.Recall();
            if (Settings.CenterMouseCursorInRecalledWindow)
                instance.CenterMouseCursorInWindow();
        }
        catch (Exception e)
        {
            return new RecallInfo(RecallResult.UnknownError, instance, e);
        }
        return new RecallInfo(RecallResult.Success, instance);
    }

    public RecallInfo Recall(int entryId)
    {
        var recallInfo = CoreRecall(entryId);

        string? errorDetails = null;
        switch(recallInfo.RecallResult)
        {
            case RecallResult.EntryNotFound:
                errorDetails = $"Entry <{entryId}> not found";
                break;
            case RecallResult.UnknownError:
                errorDetails = "Unexpected error";
                break;

        }
        if (errorDetails is not null)
        {
            if (recallInfo.Instance is null)
                Logger.Error($"Entry <{entryId}> instance recall failed. {errorDetails}");
            else
                Logger.Error($"Instance <{recallInfo.Instance.Name}> recall failed. {errorDetails}");

            if (recallInfo.RecallResult == RecallResult.UnknownError)
                Logger.Debug(recallInfo.Exception!.ToString());
        }
        else
            Logger.Info($"Instance <{recallInfo.Instance!.Name}> recalled");

        return recallInfo;
    }

    public enum UnlockMultiboxingResult
    {
        InvalidExecutablePath,
        NoProcessesFound,
        LockNotFound,
        Success,
    }

    public UnlockMultiboxingResult GlobalUnlockMultiboxing()
    {
        if (!IsGameExecutableValid())
            return UnlockMultiboxingResult.InvalidExecutablePath;

        var gameExecutableName = Path.GetFileNameWithoutExtension(Settings.GameExecutablePath);
        var processes = Process.GetProcessesByName(gameExecutableName);
        if (processes.Length == 0)
            return UnlockMultiboxingResult.NoProcessesFound;

        foreach (var p in processes)
        {
            using var app = new NativePlatform.ManagedApplication(p, LoggerRegistry.GetLogger<NativePlatform.ManagedApplication>());
            if (app.CloseNamedObject(GameInstance.CheckForOtherInstancesEventNameSuffix))
                return UnlockMultiboxingResult.Success;
        }

        return UnlockMultiboxingResult.LockNotFound;
    }
}*/