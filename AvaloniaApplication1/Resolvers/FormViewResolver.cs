using System;
using Avalonia.Controls;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Form;
using AvaloniaApplication1.Views;

namespace AvaloniaApplication1.Resolvers;

public static class FormViewResolver
{
    public static Window Resolve(IFormViewModel form)
    {
        return form switch
        {
            EditInstanceFormViewModel => new EditInstanceForm(),
            _ => throw new NotImplementedException($"Form view not mapped: {form.GetType().FullName}")
        };
    }
}