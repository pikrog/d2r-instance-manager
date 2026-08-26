using System;
using AvaloniaApplication1.HotKey.Config;

namespace AvaloniaApplication1.Common;

public sealed class HotKeyConfigValidatorProvider(IHotKeyConfigValidator validator) : IServiceProvider
{
    public object? GetService(Type serviceType) => serviceType == typeof(IHotKeyConfigValidator) ? validator : null;
}