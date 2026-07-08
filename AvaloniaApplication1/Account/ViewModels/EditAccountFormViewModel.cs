using System;
using System.ComponentModel.DataAnnotations;
using AvaloniaApplication1.Form.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Account.ViewModels;

[NotifyDataErrorInfo]
public partial class EditAccountFormViewModel : FormViewModelBase
{
    public Guid? Id { get; init; }
    
    [ObservableProperty]
    [Required]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    [Required]
    public partial string Password { get; set; } = string.Empty;
}
