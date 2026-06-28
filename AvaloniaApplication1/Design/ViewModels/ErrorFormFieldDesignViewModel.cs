using System.ComponentModel.DataAnnotations;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Form;
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