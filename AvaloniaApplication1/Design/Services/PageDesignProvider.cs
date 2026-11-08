using System;
using System.Collections.Generic;
using D2RInstanceManager.Account.ViewModels;
using D2RInstanceManager.Design.ViewModels;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.Instance.ViewModels;
using D2RInstanceManager.Log;
using D2RInstanceManager.Navigation;
using D2RInstanceManager.Region.ViewModels;

namespace D2RInstanceManager.Design.Services;

public class PageDesignProvider : IPageProvider
{
    private static readonly Dictionary<Type, Type> _pageTypes = new();

    private static void AddPageType<TViewModel, TDesignViewModel>()
    {
        _pageTypes[typeof(TViewModel)] = typeof(TDesignViewModel);
    }
    
    static PageDesignProvider()
    {
        AddPageType<InstancesPageViewModel, InstancesPageDesignViewModel>();
        AddPageType<AccountsPageViewModel, AccountsPageDesignViewModel>();
        AddPageType<RegionsPageViewModel, RegionsPageDesignViewModel>();
        AddPageType<GlobalSettingsPageViewModel, GlobalSettingsPageDesignViewModel>();
        AddPageType<LogsPageViewModel, LogsPageDesignViewModel>();
    }
    
    public PageViewModel Get(Type type) => (PageViewModel)Activator.CreateInstance(_pageTypes[type])!;
}