using System;
using D2RInstanceManager.HotKey.Config;

namespace D2RInstanceManager.Common;

public sealed class HotKeyConfigValidatorProvider(IHotKeyConfigValidator validator) : IServiceProvider
{
    public object? GetService(Type serviceType) => serviceType == typeof(IHotKeyConfigValidator) ? validator : null;
}