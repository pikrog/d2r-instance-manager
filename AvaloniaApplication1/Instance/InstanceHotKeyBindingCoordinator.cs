using System;
using AvaloniaApplication1.HotKey.Coordination;
using AvaloniaApplication1.HotKey.Platform;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Instance;

public class InstanceHotKeyBindingCoordinator : HotKeyBindingCoordinator<ShowInstanceHotKeyCommand, Guid>
{
    private readonly InstanceHotKeyConfigProvider _hotKeyConfigProvider;
    
    private readonly InstanceService _instanceService;

    public InstanceHotKeyBindingCoordinator(
        HotKeyRegistrationManager hotKeyRegistrationManager, 
        InstanceHotKeyConfigProvider hotKeyConfigProvider,
        InstanceService instanceService
        ) : base(hotKeyRegistrationManager)
    {
        _hotKeyConfigProvider = hotKeyConfigProvider;
        _instanceService = instanceService;

        _instanceService.InstanceConfigChanged += OnInstanceConfigChanged;
    }

    public void Initialize()
    {
        foreach (var binding in _hotKeyConfigProvider.GetBindings())
            Register(binding.Command, binding.KeyCombination);
    }

    private void OnInstanceConfigChanged(object? sender, InstanceConfigChangedEventArgs e)
    {
        if (e.OldSnapshot is not null)
            Unregister(e.OldSnapshot.Id);

        if (e.NewSnapshot is null)
            return;
        
        var binding = _hotKeyConfigProvider.GetBinding(e.NewSnapshot.Id);
        if (binding is null)
            return;
        Register(binding.Command, binding.KeyCombination);
    }

    protected override Action CreateAction(ShowInstanceHotKeyCommand command) => 
        () => _instanceService.Show(command.InstanceId);

    protected override Guid GetCommandId(ShowInstanceHotKeyCommand command) => command.InstanceId;
}