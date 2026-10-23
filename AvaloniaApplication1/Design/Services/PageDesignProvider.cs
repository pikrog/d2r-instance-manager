using System;
using System.Collections.Generic;
using AvaloniaApplication1.Account.ViewModels;
using AvaloniaApplication1.Design.ViewModels;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Log;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Region.ViewModels;

namespace AvaloniaApplication1.Design.Services;

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