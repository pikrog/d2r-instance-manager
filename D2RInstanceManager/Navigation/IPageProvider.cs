using System;

namespace D2RInstanceManager.Navigation;

public interface IPageProvider
{
    PageViewModel Get(Type type);
}