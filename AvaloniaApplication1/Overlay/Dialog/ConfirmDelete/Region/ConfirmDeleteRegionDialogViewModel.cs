namespace AvaloniaApplication1.Overlay.Dialog.ConfirmDelete.Region;

public class ConfirmDeleteRegionDialogViewModel(string regionName, int instanceCount) : ConfirmDeleteViewModelBase
{
    public string RegionName { get; set; } = regionName;
    
    public int InstanceCount { get; set; } = instanceCount;
    
    public bool IsUsedByInstances => InstanceCount > 0;
}