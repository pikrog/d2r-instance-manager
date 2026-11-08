using System;
using D2RInstanceManager.Dialog;
using D2RInstanceManager.Form.ViewModels;
using D2RInstanceManager.Instance.ViewModels;
using EditInstanceFormViewModel = D2RInstanceManager.Instance.ViewModels.EditInstanceFormViewModel;

namespace D2RInstanceManager.Form;

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