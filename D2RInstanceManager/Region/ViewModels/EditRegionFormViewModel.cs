using System;
using System.ComponentModel.DataAnnotations;
using D2RInstanceManager.Form.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Region.ViewModels;

[NotifyDataErrorInfo]
public partial class EditRegionFormViewModel : FormViewModelBase
{
    public Guid? Id { get; init; }
    
    [ObservableProperty]
    [Required]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    [Required]
    public partial string Address { get; set; } = string.Empty;
}
