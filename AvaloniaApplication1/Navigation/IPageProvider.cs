using System;

namespace AvaloniaApplication1.Navigation;

public interface IPageProvider
{
    PageViewModel Get(Type type);
}