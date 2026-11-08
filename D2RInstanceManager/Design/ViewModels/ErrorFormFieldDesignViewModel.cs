using System.ComponentModel.DataAnnotations;
using D2RInstanceManager.Form.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Design.ViewModels;

public partial class ErrorFormFieldDesignViewModel : FormViewModelBase
{
    [Required]
    [NotifyDataErrorInfo]
    [ObservableProperty]
    public partial string Error { get; set; } = string.Empty;

    public ErrorFormFieldDesignViewModel()
    {
        Validate();
    }
}