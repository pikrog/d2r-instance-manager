using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaApplication1.ViewModels;

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

        var name = param.GetType().FullName!
            .Replace("Design.", "", StringComparison.Ordinal)
            .Replace("DesignViewModel", "ViewModel", StringComparison.Ordinal)
            .Replace("ViewModel", "View", StringComparison.Ordinal);
        
        var lastDot = name.LastIndexOf('.');
        var baseViewName = name[(lastDot + 1)..];
        
        const string viewsSubPath = ".Views";
        var baseNamespaceLength = name.IndexOf(viewsSubPath, StringComparison.Ordinal) + viewsSubPath.Length;
        var baseNamespace = name[..baseNamespaceLength];
        var fullViewName = $"{baseNamespace}.{baseViewName}";
            
        var type = Type.GetType(fullViewName);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + fullViewName };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    /*[GeneratedRegex(@"Views\.(.+\.).+View")]
    private static partial Regex ViewsSubPathRegex { get; }*/
}