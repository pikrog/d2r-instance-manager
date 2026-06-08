using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public partial class RegionCardViewModel(Guid? id = null, string name = "", string address = "") : ViewModelBase
{
    [ObservableProperty]
    public partial EditRegionFormViewModel? EditForm { get; private set; }
    
    public Guid? Id { get; } = id;
    
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
