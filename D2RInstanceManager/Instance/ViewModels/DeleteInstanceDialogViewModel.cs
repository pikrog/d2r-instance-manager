using D2RInstanceManager.Overlay.Dialog.Delete;

namespace D2RInstanceManager.Instance.ViewModels;

public class DeleteInstanceDialogViewModel(string instanceName) : DeleteDialogViewModelBase
{
    public string InstanceName { get; } = instanceName;
}