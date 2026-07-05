using System;
using AvaloniaApplication1.ViewModels.Card;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels.Region;

public partial class RegionCardViewModel(Guid? id = null, string name = "", string address = "") : CardViewModel
{
    [ObservableProperty]
    public partial EditRegionFormViewModel? EditForm { get; private set; }
    
    public Guid? Id { get; set; } = id;
    
    [ObservableProperty]
    public partial string Name { get; set; } = name;
    
    [ObservableProperty]
    public partial string Address { get; set; } = address;

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
