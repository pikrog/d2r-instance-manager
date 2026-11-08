using System;
using D2RInstanceManager.Account;
using D2RInstanceManager.Authentication.Models;
using D2RInstanceManager.Common;
using D2RInstanceManager.Config;
using D2RInstanceManager.Display;
using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Helpers.ProcessStop;
using D2RInstanceManager.Engine.Models.Contexts.Launch;
using D2RInstanceManager.GameExecutable;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.Instance.Models;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Instance;

using LaunchPreparation = Result<EngineLaunchContext, InstanceLaunchFailure>;

public class InstanceLaunchContextFactory(
    ConfigService configService,
    AccountSnapshotTransformer accountSnapshotTransformer,
    IGameExecutableFileValidator gameExecutableFileValidator,
    ILogger<InstanceLaunchContextFactory> logger)
{
    public LaunchPreparation Create(InstanceSnapshot instance)
    {
        var settings = configService.Config.GetGlobalSettings();

        var authenticationContext = CreateAuthenticationContext(instance);

        var displayResolution = DisplayResolver.ResolveDisplayId(instance.Display, 
            settings.FallbackToPrimaryDisplayIfInvalid);
        
        if (displayResolution is not { Id: var displayId, IsFallback: var isFallback })
            return LaunchPreparation.Failure(InstanceLaunchFailure.DisplayUnavailable);

        if (isFallback)
            LogDisplayFallback(displayId);

        var executablePath = settings.GameExecutablePath;
        logger.LogTrace("Executable path: {GameExecutablePath}", executablePath);

        if (ValidateExecutable(executablePath) is { } failure)
            return LaunchPreparation.Failure(failure);

        var instanceContext = new InstanceLaunchContext(
            executablePath,
            authenticationContext,
            displayId,
            instance.IsNoSound,
            instance.IsWindowedMode);

        return LaunchPreparation.Success(
            new EngineLaunchContext(instanceContext, CreateEnginePolicies(settings)));
    }

    private AuthenticationContext CreateAuthenticationContext(InstanceSnapshot instance)
    {
        if (!instance.IsOnlineMode)
            return new OfflineAuthenticationContext();

        var accountConfigSnapshot = configService.Config.GetAccount(instance.AccountId!.Value);
        var account = accountSnapshotTransformer.ToRuntimeSnapshot(accountConfigSnapshot);
        
        var region = configService.Config.GetRegion(instance.RegionId!.Value);

        return instance.AuthenticationMethod switch
        {
            AuthenticationMethod.CommandLineArguments =>
                new CliAuthenticationContext(account.Username, account.Password, region.Address),
            AuthenticationMethod.OsiTokenRegistry =>
                new OsiAuthenticationContext(region.Address),
            _ => throw new InvalidOperationException(
                $"Unknown authentication method {instance.AuthenticationMethod}")
        };
    }

    private void LogDisplayFallback(string displayId)
    {
        var cached = configService.Config.GetDisplay(displayId);
        logger.LogWarning(
            "Display {DisplayDescription} selected for this instance is not available, falling back to primary display",
            $"{cached.Description} [{cached.Width}x{cached.Height}]");
    }

    private InstanceLaunchFailure? ValidateExecutable(string path)
    {
        var result = gameExecutableFileValidator.Validate(path);

        if (result.FileMetadata is { } d)
        {
            logger.LogTrace(
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

        switch (result.Code)
        {
            case GameExecutableFileValidationResultCode.Ok:
                return null;
            case GameExecutableFileValidationResultCode.UnrecognizedExecutable:
                logger.LogWarning("The executable was found but its metadata does not match the expected game");
                return null;
            case GameExecutableFileValidationResultCode.MissingPath:
                logger.LogError("The game executable path is missing");
                return InstanceLaunchFailure.ExecutablePathMissing;
            case GameExecutableFileValidationResultCode.FileNotFound:
                logger.LogError("The game executable file was not found: {GameExecutablePath}", path);
                return InstanceLaunchFailure.ExecutableNotFound;
            case GameExecutableFileValidationResultCode.InvalidExecutableFormat:
                logger.LogError("The game executable file has an invalid format: {GameExecutablePath}", path);
                return InstanceLaunchFailure.ExecutableInvalidFormat;
            default:
                throw new InvalidOperationException($"Unknown game executable file validation result {result}");
        }
    }

    private static EnginePolicies CreateEnginePolicies(GlobalSettingsSnapshot settings)
    {
        var multiboxRetry = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.UnlockMultiboxRetryDelayMs),
            settings.UnlockMultiboxMaxRetries);

        var gracefulStopRetry = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.GracefulInstanceCloseTimeoutMs),
            settings.GracefulInstanceCloseRetries);

        return new EnginePolicies(
            multiboxRetry,
            new ProcessStopPolicies(gracefulStopRetry, 
                TimeSpan.FromMilliseconds(settings.ForcefulInstanceCloseTimeoutMs))
        );
    }
}