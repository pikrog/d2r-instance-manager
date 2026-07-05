using System;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Common.Dialog;
using AvaloniaApplication1.ViewModels.Common.Form;
using AvaloniaApplication1.ViewModels.Instance;
using EditInstanceFormViewModel = AvaloniaApplication1.ViewModels.Instance.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Resolvers;

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