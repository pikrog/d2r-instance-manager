using System;
using System.ComponentModel.DataAnnotations;
using D2RInstanceManager.Form.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Account.ViewModels;

public partial class EditAccountFormViewModel : FormViewModelBase
{
    public Guid? Id { get; init; }
    
    [ObservableProperty]
    public partial string? DisplayName { get; set; }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string Password { get; set; } = string.Empty;
}
