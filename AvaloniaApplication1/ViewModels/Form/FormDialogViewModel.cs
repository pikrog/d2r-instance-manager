using System;
using AvaloniaApplication1.ViewModels.Dialog;

namespace AvaloniaApplication1.ViewModels.Form;

public class FormDialogViewModel<TForm>(TForm form) : FormHostViewModel<TForm>(form), IDialogAware 
    where TForm : FormViewModelBase
{
    public Action<bool>? CloseDialog { get; set; }
    
    protected override void OnSaved()
    {
        CloseDialog?.Invoke(true);
    }

    protected override void OnCanceled()
    {
        CloseDialog?.Invoke(false);
    }
}
