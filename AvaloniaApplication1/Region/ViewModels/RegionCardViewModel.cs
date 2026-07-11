using System;
using AvaloniaApplication1.Card.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Region.ViewModels;

public partial class RegionCardViewModel : CardViewModel
{
    public RegionCardViewModel(Guid? id, string name, string address, int instancesCount)
    {
        Id = id;
        Name = name;
        Address = address;
        InstancesCount = instancesCount;
    }

    public RegionCardViewModel() { }

    [ObservableProperty]
    public partial EditRegionFormViewModel? EditForm { get; private set; }
    
    public Guid? Id { get; set; }
    
    [ObservableProperty]
    public partial string Name { get; set; }
    
    [ObservableProperty]
    public partial string Address { get; set; }

    [ObservableProperty] 
    public partial int InstancesCount { get; set; }

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
