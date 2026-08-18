using System;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Navigation;

public class PageProvider(IServiceProvider serviceProvider) : IPageProvider
{
    public PageViewModel Get(Type type)
    {
        if (!type.IsAssignableTo(typeof(PageViewModel)))
            throw new ArgumentException("Type must be a PageViewModel", nameof(type));
        return (PageViewModel)serviceProvider.GetRequiredService(type);
    }
}