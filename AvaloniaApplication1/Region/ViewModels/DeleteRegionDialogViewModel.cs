using AvaloniaApplication1.Overlay.Dialog.Delete;

namespace AvaloniaApplication1.Region.ViewModels;

public class DeleteRegionDialogViewModel(string regionName, int instanceCount) : DeleteDialogViewModelBase
{
    public string RegionName { get; } = regionName;
    
    public int InstanceCount { get; } = instanceCount;
    
    public bool IsUsedByInstances => InstanceCount > 0;
}