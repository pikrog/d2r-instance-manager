using System;
using D2RInstanceManager.Card.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Region.ViewModels;

public partial class RegionCardViewModel : CardViewModel
{
    public RegionCardViewModel(Guid? id, string name, string address, int instanceCount)
    {
        Id = id;
        Name = name;
        Address = address;
        InstanceCount = instanceCount;
    }

    public RegionCardViewModel()
    {
        Name = string.Empty;
        Address = string.Empty;
    }

    [ObservableProperty]
    public partial EditRegionFormViewModel? EditForm { get; private set; }
    
    public Guid? Id { get; set; }
    
    [ObservableProperty]
    public partial string Name { get; set; }
    
    [ObservableProperty]
    public partial string Address { get; set; }

    [ObservableProperty] 
    public partial int InstanceCount { get; set; }

    public void OpenForm()
    {
        EditForm = new EditRegionFormViewModel
        {
            Id = Id,
            Name = Name,
            Address = Address
        };
    }
    
    public void CloseForm()
    {
        EditForm = null;
    }
}
