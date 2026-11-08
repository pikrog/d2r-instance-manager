using System;
using D2RInstanceManager.HotKey.Coordination;
using D2RInstanceManager.HotKey.Platform;
using D2RInstanceManager.Initialization;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.EventArgs;

namespace D2RInstanceManager.Instance;

public class InstanceHotKeyBindingCoordinator : 
    HotKeyBindingCoordinator<ShowInstanceHotKeyCommand, Guid>,
    IRuntimeInitializer
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
    }

    public void Initialize()
    {
        _instanceService.InstanceConfigChanged += OnInstanceConfigChanged;
        
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