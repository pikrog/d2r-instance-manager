using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.HotKey.Config;
using AvaloniaApplication1.HotKey.Coordination;
using AvaloniaApplication1.Instance.Models;

namespace AvaloniaApplication1.Instance;

public class InstanceHotKeyConfigProvider(ConfigService configService) : IHotKeyConfigProvider<ShowInstanceHotKeyCommand>
{
    public IReadOnlyList<HotKeyBinding<ShowInstanceHotKeyCommand>> GetBindings() =>
        configService.Config.GetAllInstances()
            .Where(i => i.ShowCommandHotKey.Key != Key.None)
            .Select(i => new HotKeyBinding<ShowInstanceHotKeyCommand>(i.ShowCommandHotKey, new ShowInstanceHotKeyCommand(i.Id)))
            .ToList();

    public HotKeyBinding<ShowInstanceHotKeyCommand>? GetBinding(Guid instanceId)
    {
        var instance = configService.Config.GetInstance(instanceId);
        return instance.ShowCommandHotKey.Key != Key.None 
            ? new HotKeyBinding<ShowInstanceHotKeyCommand>(instance.ShowCommandHotKey, new ShowInstanceHotKeyCommand(instanceId)) 
            : null;
    }
}