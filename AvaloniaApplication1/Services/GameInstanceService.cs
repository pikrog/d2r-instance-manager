using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Helpers;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.Mappers;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;
using DynamicData.Kernel;

namespace AvaloniaApplication1.Services;

public class GameInstanceService(ConfigService configService, GameInstanceManager gameInstanceManager, AccountService accountService, RegionService regionService, DisplayService displayService)
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

    public event Action<Guid>? InstanceStateChanged
    {
        add => gameInstanceManager.InstanceStateChanged += value;
        remove => gameInstanceManager.InstanceStateChanged -= value;
    }

    private async Task AddAsync(GameInstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.AddInstance(snapshot));
        gameInstanceManager.Register(snapshot.Id);
    }

    private async Task UpdateAsync(GameInstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.UpdateInstance(snapshot));
    }

    public async Task SaveAsync(GameInstanceDraft draft)
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
            draft.Display,
            draft.IsNoSound,
            draft.IsWindowedMode,
            draft.RecallHotKey
        );

        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }
    
    public async Task RemoveAsync(Guid id)
    {
        await configService.ChangeAsync(context => context.RemoveInstance(id));
        gameInstanceManager.Remove(id);
    }

    public GameInstanceSnapshot GetInstanceConfigSnapshot(Guid id)
    {
        return configService.Config.GetInstance(id);
    }

    public RuntimeSnapshot GetInstanceRuntimeSnapshot(Guid id)
    {
        return gameInstanceManager.GetRuntimeSnapshot(id);
    }

    public GlobalSettingsSnapshot GetSettingsSnapshot()
    {
        return configService.Config.GetGlobalSettings();
    }

    public GameInstanceTableRow GetTableRow(Guid id)
    {
        var configSnapshot = GetInstanceConfigSnapshot(id);
        var runtimeSnapshot = GetInstanceRuntimeSnapshot(id);
        var status = GameInstanceStatusMapper.Map(runtimeSnapshot);
        return new GameInstanceTableRow(id, configSnapshot.Name, status, runtimeSnapshot.IsActive);
    }

    public IReadOnlyList<GameInstanceTableRow> GetTable()
    {
        var instances = gameInstanceManager.GetAllRuntimeStates().ToDictionary(i => i.Id);
        return configService.Config.GetAllInstances().Select(i =>
            {
                instances.TryGetValue(i.Id, out var runtimeSnapshot);
                var status = runtimeSnapshot is not null  
                    ? GameInstanceStatusMapper.Map(runtimeSnapshot) 
                    : GameInstanceStatus.Unknown; // todo: throw or ignore?
                var isActive = runtimeSnapshot?.IsActive ?? false; // ?
                return new GameInstanceTableRow(i.Id, i.Name, status, isActive);
            }).ToList();
    }

    public async Task LaunchAsync(Guid id)
    {
        var settings = GetSettingsSnapshot();
        
        var snapshot = GetInstanceConfigSnapshot(id);
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

        var displayId = displayService.ResolveDisplayId(snapshot.Display);
        if (displayId is null)
            throw new InvalidOperationException("Failed to resolve display id"); // todo: return Result<Unit, Error>
        
        // todo: check if path is valid
        
        var instanceLaunchContext = new InstanceLaunchContext(
            settings.GameExecutablePath,
            authenticationContext,
            displayId,
            snapshot.IsNoSound,
            snapshot.IsWindowedMode
        );

        var multiboxUnlockRetryPolicy = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.UnlockMultiboxRetryDelayMs),
            settings.UnlockMultiboxMaxRetries
        );

        var gracefulInstanceStopRetryPolicy = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.GracefulInstanceCloseTimeoutMs),
            settings.GracefulInstanceCloseRetries
        );
        
        var forcefulInstanceStopTimeout = TimeSpan.FromMilliseconds(settings.ForcefulInstanceCloseTimeoutMs);
        
        var processStopPolicies = new ProcessStopPolicies(gracefulInstanceStopRetryPolicy, forcefulInstanceStopTimeout);
        
        var enginePolicies = new EnginePolicies(
            multiboxUnlockRetryPolicy,
            processStopPolicies
        );
        
        var engineLaunchContext = new EngineLaunchContext(instanceLaunchContext, enginePolicies);
        
        await gameInstanceManager.LaunchAsync(snapshot.Id, engineLaunchContext);
    }

    public async Task StopAsync(Guid id) => await gameInstanceManager.StopAsync(id);

    public void Show(Guid id) => gameInstanceManager.Show(id);
}