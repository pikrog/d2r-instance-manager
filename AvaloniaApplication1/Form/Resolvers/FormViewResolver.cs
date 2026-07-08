using System;
using Avalonia.Controls;
using AvaloniaApplication1.Form.ViewModels;
using EditInstanceFormView = AvaloniaApplication1.Instance.Views.EditInstanceFormView;
using EditInstanceFormViewModel = AvaloniaApplication1.Instance.ViewModels.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Form.Resolvers;

public static class FormViewResolver
{
    public static Window Resolve(IFormViewModel form)
    {
        return form switch
        {
            EditInstanceFormViewModel => new EditInstanceFormView(),
            _ => throw new NotImplementedException($"Form view not mapped: {form.GetType().FullName}")
        };
    }
}