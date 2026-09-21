using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;
using AvaloniaApplication1.Log;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Instance;

public class InstanceService
{
    private readonly ConfigService _configService;

    private readonly InstanceConfigValidator _validator;

    private readonly InstanceManager _instanceManager;

    private readonly InstanceNameRegistry _instanceNameRegistry;

    private readonly ILogger<InstanceService> _logger;

    public event EventHandler<RuntimeSnapshot>? InstanceStateChanged;
    
    public event EventHandler<InstanceConfigChangedEventArgs>? InstanceConfigChanged;

    private readonly Dictionary<Guid, bool> _instanceActivityStates = [];

    public InstanceService(ConfigService configService,
        InstanceConfigValidator validator,
        InstanceManager instanceManager,
        InstanceNameRegistry instanceNameRegistry,
        ILogger<InstanceService> logger)
    {
        _configService = configService;
        _validator = validator;
        _instanceManager = instanceManager;
        _instanceNameRegistry = instanceNameRegistry;
        _logger = logger;

        _instanceManager.InstanceStateChanged += OnInstanceStateChanged;
    }

    private void OnInstanceStateChanged(object? sender, RuntimeSnapshot snapshot)
    {
        var previousActivityState = _instanceActivityStates.GetValueOrDefault(snapshot.Id);
        _instanceActivityStates[snapshot.Id] = snapshot.IsActive;

        if (previousActivityState && !snapshot.IsActive)
        {
            var instanceName = _instanceNameRegistry.Get(snapshot.Id);
            using var _ = _logger.BeginScope(new Dictionary<string, object?>{{ LogProperties.InstanceName, instanceName }});
            _logger.LogInformation("Instance stopped");
        }

        InstanceStateChanged?.Invoke(this, snapshot);
    }

    private async Task AddAsync(InstanceSnapshot snapshot)
    {
        await _configService.ChangeAsync(context => context.AddInstance(snapshot));
        _instanceNameRegistry.Set(snapshot.Id, snapshot.Name);
        _instanceManager.Register(snapshot.Id);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, newSnapshot: snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    private async Task UpdateAsync(InstanceSnapshot snapshot)
    {
        var oldSnapshot = GetConfigSnapshot(snapshot.Id);

        await _configService.ChangeAsync(context => context.UpdateInstance(snapshot));
        _instanceNameRegistry.Set(snapshot.Id, snapshot.Name);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, oldSnapshot, snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    public async Task SaveAsync(InstanceDraft draft)
    {
        // todo:
        // Validate(draft);

        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new InstanceSnapshot(
            id,
            draft.Name,
            draft.IsOnlineMode,
            draft.AccountId,
            draft.AuthenticationMethod,
            draft.RegionId,
            draft.Display,
            draft.IsNoSound,
            draft.IsWindowedMode,
            draft.ShowCommandHotKey
        );

        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }
    
    public async Task RemoveAsync(Guid id)
    {
        var snapshot = GetConfigSnapshot(id);

        await _configService.ChangeAsync(context => context.RemoveInstance(id));
        _instanceNameRegistry.Remove(id);
        _instanceManager.Remove(id);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    public InstanceSnapshot GetConfigSnapshot(Guid id)
    {
        return _configService.Config.GetInstance(id);
    }

    public IReadOnlyList<InstanceSnapshot> GetConfigSnapshots()
    {
        return _configService.Config.GetAllInstances();
    }

    public RuntimeSnapshot GetRuntimeSnapshot(Guid id)
    {
        return _instanceManager.GetRuntimeSnapshot(id);
    }

    private InstanceSummary CreateSummary(InstanceSnapshot snapshot, RuntimeSnapshot runtimeSnapshot)
    {
        var status = InstanceStatusMapper.Map(runtimeSnapshot);
        var issues = _validator.Validate(snapshot);
        return new InstanceSummary(snapshot.Id, snapshot.Name, status, runtimeSnapshot.IsActive, issues);
    }

    public InstanceSummary GetSummary(Guid id)
    {
        var configSnapshot = GetConfigSnapshot(id);
        var runtimeSnapshot = GetRuntimeSnapshot(id);
        return CreateSummary(configSnapshot, runtimeSnapshot);
    }

    public virtual IReadOnlyList<InstanceSummary> GetSummaries()
    {
        var instances = _instanceManager.GetAllRuntimeStates().ToDictionary(i => i.Id);
        return _configService.Config.GetAllInstances().Select(i =>
            {
                var runtimeSnapshot = instances[i.Id];
                return CreateSummary(i, runtimeSnapshot);
            }).ToList();
    }

    public Task<int> GetActiveCountAsync() => _instanceManager.GetActiveCountAsync();
    
    private AuthenticationContext GetAuthenticationContext(InstanceSnapshot instance)
    {
        if (!instance.IsOnlineMode)
            return new OfflineAuthenticationContext();

        var account = _configService.Config.GetAccount(instance.AccountId!.Value);
        var region = _configService.Config.GetRegion(instance.RegionId!.Value);

        return instance.AuthenticationMethod switch
        {
            AuthenticationMethod.CommandLineArguments => new CliAuthenticationContext(account.Username, account.Password, region.Address),
            AuthenticationMethod.OsiTokenRegistry => new OsiAuthenticationContext(region.Address),
            _ => throw new InvalidOperationException($"Unknown authentication method {instance.AuthenticationMethod}")
        };
    }
    
    private EnginePolicies CreateEnginePolicies(GlobalSettingsSnapshot settings)
    {
        var multiboxRetry = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.UnlockMultiboxRetryDelayMs),
            settings.UnlockMultiboxMaxRetries);

        var gracefulStopRetry = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.GracefulInstanceCloseTimeoutMs),
            settings.GracefulInstanceCloseRetries);

        return new EnginePolicies(
            multiboxRetry,
            new ProcessStopPolicies(gracefulStopRetry, TimeSpan.FromMilliseconds(settings.ForcefulInstanceCloseTimeoutMs))
        );
    }

    private bool ValidateGameExecutableFile(string path)
    {
        var fileValidationResult = GameExecutableFileValidator.Validate(path);

        if (fileValidationResult.FileMetadata is { } d)
        {
            _logger.LogTrace(
                """
                File metadata: 
                    Company Name={CompanyName}, 
                    Product Name={ProductName}, 
                    Description={Description}, 
                    Version={Version}
                """,
                d.CompanyName,
                d.ProductName,
                d.Description,
                d.Version);
        }

        switch (fileValidationResult.Code)
        {
            case GameExecutableFileValidator.ValidateResultCode.Ok:
                return true;
            case GameExecutableFileValidator.ValidateResultCode.MissingPath:
                _logger.LogError("The game executable path is missing");
                return false;
            case GameExecutableFileValidator.ValidateResultCode.FileNotFound:
                _logger.LogError("The game executable file was not found: {GameExecutablePath}", path);
                return false;
            case GameExecutableFileValidator.ValidateResultCode.InvalidExecutableFormat:
                _logger.LogError("The game executable file has an invalid format: {GameExecutablePath}", path);
                return false;
            case GameExecutableFileValidator.ValidateResultCode.UnrecognizedExecutable:
                _logger.LogWarning("The executable was found but its metadata does not match the expected game");
                return true;
            default:
                throw new InvalidOperationException($"Unknown game executable file validation result {fileValidationResult}");
        }
    }

    public async Task LaunchAsync(Guid id)
    {
        var settings = _configService.Config.GetGlobalSettings();
        
        var snapshot = GetConfigSnapshot(id);
        var authenticationContext = GetAuthenticationContext(snapshot);

        using var _ = _logger.BeginScope(new Dictionary<string, object> {{ LogProperties.InstanceName, snapshot.Name }});
        
        var displayResolution = DisplayResolver.ResolveDisplayId(snapshot.Display, settings.FallbackToPrimaryDisplayIfInvalid);
        if (displayResolution is not { Id: var displayId, IsFallback: var isFallback })
            return; // todo: return error

        if (isFallback)
        {
            var cachedDisplay = _configService.Config.GetCachedDisplay(displayId);
            var displayDescription = $"{cachedDisplay.Description} [{cachedDisplay.Width}x{cachedDisplay.Height}]";
            _logger.LogWarning(
                "Display {DisplayDescription} selected for this instance is not available, falling back to primary display",
                displayDescription);
        }
        
        var executablePath = settings.GameExecutablePath;
        
        _logger.LogTrace("Executable path: {GameExecutablePath}", executablePath);
        
        if (!ValidateGameExecutableFile(executablePath))
            return; // todo: return error
        
        var instanceLaunchContext = new InstanceLaunchContext(
            executablePath,
            authenticationContext,
            displayId,
            snapshot.IsNoSound,
            snapshot.IsWindowedMode
        );
        
        var enginePolicies = CreateEnginePolicies(settings);
        
        var engineLaunchContext = new EngineLaunchContext(instanceLaunchContext, enginePolicies);
        
        await _instanceManager.LaunchAsync(snapshot.Id, engineLaunchContext);
        
        _logger.LogInformation("Instance launched");
    }

    public async Task StopAsync(Guid id) => await _instanceManager.StopAsync(id);

    public Task<IReadOnlyList<ShutdownRequest>> RequestGracefulShutdownAllAsync() => 
        _instanceManager.RequestGracefulShutdownAllAsync();

    public void Show(Guid id) => _instanceManager.Show(id);
}
