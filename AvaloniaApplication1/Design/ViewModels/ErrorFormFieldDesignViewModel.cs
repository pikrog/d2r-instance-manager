using System.ComponentModel.DataAnnotations;
using AvaloniaApplication1.Form.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Design.ViewModels;

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