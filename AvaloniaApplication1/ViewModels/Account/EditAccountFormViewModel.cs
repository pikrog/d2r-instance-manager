using System;
using System.ComponentModel.DataAnnotations;
using AvaloniaApplication1.ViewModels.Common.Form;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels.Account;

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
