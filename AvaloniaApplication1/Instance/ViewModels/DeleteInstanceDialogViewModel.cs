using AvaloniaApplication1.Overlay.Dialog.Delete;

namespace AvaloniaApplication1.Instance.ViewModels;

public class DeleteInstanceDialogViewModel(string instanceName) : DeleteDialogViewModelBase
{
    public string InstanceName { get; } = instanceName;
}