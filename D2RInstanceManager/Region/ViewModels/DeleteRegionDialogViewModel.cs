using D2RInstanceManager.Overlay.Dialog.Delete;

namespace D2RInstanceManager.Region.ViewModels;

public class DeleteRegionDialogViewModel(string regionName, int instanceCount) : DeleteDialogViewModelBase
{
    public string RegionName { get; } = regionName;
    
    public int InstanceCount { get; } = instanceCount;
    
    public bool IsUsedByInstances => InstanceCount > 0;
}