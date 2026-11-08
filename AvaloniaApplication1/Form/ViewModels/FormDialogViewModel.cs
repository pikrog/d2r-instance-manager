using System;
using D2RInstanceManager.Dialog;

namespace D2RInstanceManager.Form.ViewModels;

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
