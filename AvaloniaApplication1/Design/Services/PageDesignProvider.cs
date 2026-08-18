using System;
using System.Collections.Generic;
using AvaloniaApplication1.Account.ViewModels;
using AvaloniaApplication1.Design.ViewModels;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Region.ViewModels;

namespace AvaloniaApplication1.Design.Services;

public class PageDesignProvider : IPageProvider
{
    private readonly Dictionary<Type, Type> _pageTypes = new();

    private void AddPageType<TViewModel, TDesignViewModel>()
    {
        _pageTypes[typeof(TViewModel)] = typeof(TDesignViewModel);
    }
    
    
    public PageDesignProvider()
    {
        AddPageType<InstancesPageViewModel, InstancesPageDesignViewModel>();
        AddPageType<AccountsPageViewModel, AccountsPageDesignViewModel>();
        AddPageType<RegionsPageViewModel, RegionsPageDesignViewModel>();
        AddPageType<GlobalSettingsPageViewModel, GlobalSettingsPageDesignViewModel>();
    }
    
    public PageViewModel Get(Type type) => (PageViewModel)Activator.CreateInstance(_pageTypes[type])!;
}