using System;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Form.ViewModels;
using AvaloniaApplication1.Instance.ViewModels;
using EditInstanceFormViewModel = AvaloniaApplication1.Instance.ViewModels.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Form;

public static class FormDialogViewModelResolver
{
    public static IDialogAware Resolve(IFormViewModel form)
    {
        return form switch
        {
            EditInstanceFormViewModel vm => new EditInstanceFormDialogViewModel(vm),
            _ => throw new NotImplementedException($"Form dialog view model not mapped: {form.GetType().FullName}")
        };
    }
}