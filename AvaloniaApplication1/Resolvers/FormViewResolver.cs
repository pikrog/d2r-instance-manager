using System;
using Avalonia.Controls;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Common.Form;
using AvaloniaApplication1.Views;
using EditInstanceFormViewModel = AvaloniaApplication1.ViewModels.Instance.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Resolvers;

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