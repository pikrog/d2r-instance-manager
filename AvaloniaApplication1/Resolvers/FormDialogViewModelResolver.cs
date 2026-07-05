using System;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Dialog;
using AvaloniaApplication1.ViewModels.Form;

namespace AvaloniaApplication1.Mappers;

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