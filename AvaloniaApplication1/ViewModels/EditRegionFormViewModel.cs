using System;
using System.ComponentModel.DataAnnotations;
using AvaloniaApplication1.ViewModels.Form;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

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
