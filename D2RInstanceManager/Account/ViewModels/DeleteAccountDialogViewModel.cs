using D2RInstanceManager.Overlay.Dialog.Delete;

namespace D2RInstanceManager.Account.ViewModels;

public class DeleteAccountDialogViewModel(string? displayName, string username, int instanceCount) 
    : DeleteDialogViewModelBase
{
    public string? DisplayName { get; } = displayName;
    
    public string Username { get; } = username;
    
    public bool HasDisplayName => DisplayName is not null;
    
    public int InstanceCount { get; } = instanceCount;
    
    public bool IsUsedByInstances => InstanceCount > 0;
}