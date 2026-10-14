using System;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Instance;

using LaunchPreparation = Result<EngineLaunchContext, InstanceLaunchFailure>;

public class InstanceLaunchContextFactory(
    ConfigService configService,
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

        var account = configService.Config.GetAccount(instance.AccountId!.Value);
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
        var cached = configService.Config.GetCachedDisplay(displayId);
        logger.LogWarning(
            "Display {DisplayDescription} selected for this instance is not available, falling back to primary display",
            $"{cached.Description} [{cached.Width}x{cached.Height}]");
    }

    private InstanceLaunchFailure? ValidateExecutable(string path)
    {
        var result = GameExecutableFileValidator.Validate(path);

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
            case GameExecutableFileValidator.ValidateResultCode.Ok:
                return null;
            case GameExecutableFileValidator.ValidateResultCode.UnrecognizedExecutable:
                logger.LogWarning("The executable was found but its metadata does not match the expected game");
                return null;
            case GameExecutableFileValidator.ValidateResultCode.MissingPath:
                logger.LogError("The game executable path is missing");
                return InstanceLaunchFailure.ExecutablePathMissing;
            case GameExecutableFileValidator.ValidateResultCode.FileNotFound:
                logger.LogError("The game executable file was not found: {GameExecutablePath}", path);
                return InstanceLaunchFailure.ExecutableNotFound;
            case GameExecutableFileValidator.ValidateResultCode.InvalidExecutableFormat:
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