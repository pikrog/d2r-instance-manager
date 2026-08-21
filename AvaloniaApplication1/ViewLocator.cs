using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using AvaloniaApplication1.Common;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public partial class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var viewModelType = param.GetType();

        while (viewModelType is not null && viewModelType.IsAssignableTo(typeof(IViewModel)))
        {
            var viewName = viewModelType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            
            var viewType = Type.GetType(viewName);

            if (viewType is not null)
            {
                return (Control)ActivatorUtilities.CreateInstance(App.Services, viewType);
            }
            
            viewModelType = viewModelType.BaseType;
        }

        return new TextBlock { Text = $"View not found for {param.GetType().FullName}", TextWrapping = TextWrapping.Wrap};
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}