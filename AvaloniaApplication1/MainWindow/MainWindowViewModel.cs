using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using D2RInstanceManager.Common;
using D2RInstanceManager.Dialog;
using D2RInstanceManager.Instance;
using D2RInstanceManager.Instance.ViewModels;
using D2RInstanceManager.Navigation;
using D2RInstanceManager.Notification;
using D2RInstanceManager.Overlay;
using D2RInstanceManager.Overlay.Dialog.ShutdownProgress;
using D2RInstanceManager.Overlay.Dialog.StopInstances;
using D2RInstanceManager.Toast;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace D2RInstanceManager.MainWindow;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    public IPageHost PageHost { get; }

    public IOverlayHost OverlayHost { get; }
    
    public IToastHost ToastHost { get; }
    
    private readonly NavigationService _navigationService;
    
    private readonly OverlayService _overlayService;
    
    private readonly InstanceService _instanceService;
    
    private readonly INotificationSource _instancesNotificationSource;
    
    private readonly INotificationSource _logsNotificationSource;

    [ObservableProperty]
    public partial int InstancesNotificationCount { get; private set; }
    
    [ObservableProperty]
    public partial int LogsNotificationCount { get; private set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSingleInstanceActive), nameof(AreMultipleInstancesActive))]
    public partial int ActiveInstancesCount { get; private set; }

    public bool IsSingleInstanceActive => ActiveInstancesCount == 1;
    
    public bool AreMultipleInstancesActive => ActiveInstancesCount > 1;
    
    public MainWindowViewModel(IPageHost pageHost,
        NavigationService navigationService,
        IOverlayHost overlayHost,
        OverlayService overlayService,
        IToastHost toastHost,
        InstanceService instanceService,
        [FromKeyedServices(NotificationCounterKey.Instances)] INotificationSource instancesNotificationSource,
        [FromKeyedServices(NotificationCounterKey.Logs)] INotificationSource logsNotificationSource)
    {
        _navigationService = navigationService;
        _overlayService = overlayService;
        _instanceService = instanceService;
        _instancesNotificationSource = instancesNotificationSource;
        _logsNotificationSource = logsNotificationSource;
        
        PageHost = pageHost;
        OverlayHost = overlayHost;
        ToastHost = toastHost;

        _instanceService.ActiveCountChanged += (_, _) => Dispatcher.UIThread.Post(UpdateActiveInstancesCount);
        _instancesNotificationSource.CountChanged += (_, _) => Dispatcher.UIThread.Post(UpdateInstancesNotificationCount);
        _logsNotificationSource.CountChanged += (_, _) => Dispatcher.UIThread.Post(UpdateLogsNotificationCount);
        
        UpdateActiveInstancesCount();
        UpdateInstancesNotificationCount();
        UpdateLogsNotificationCount();
    }

    private void UpdateActiveInstancesCount()
    {
        ActiveInstancesCount = _instanceService.CachedActiveCount;
    }

    private void UpdateInstancesNotificationCount()
    {
        InstancesNotificationCount = _instancesNotificationSource.Count;
    }

    private void UpdateLogsNotificationCount()
    {
        LogsNotificationCount = _logsNotificationSource.Count;
    }

    public async Task SetupAsync() => await _navigationService.NavigateAsync<InstancesPageViewModel>();

    [RelayCommand]
    private async Task SetPage(Type pageType) => await _navigationService.NavigateAsync(pageType);
    
    public async Task<bool> TryExitAsync()
    {
        var currentPage = PageHost.CurrentPage;
        if (currentPage is not null && !await currentPage.CanLeaveAsync())
            return false;

        var activeInstanceCount = await _instanceService.GetActiveCountAsync();
        if (activeInstanceCount == 0)
        {
            if (currentPage is not null)
                await currentPage.OnLeaveAsync();
            return true;
        }
        
        var stopInstancesDialog = new StopInstancesDialogViewModel(activeInstanceCount);
        var result = await _overlayService.ShowAsync(stopInstancesDialog);
        if (result == StopInstancesAction.Cancel)
            return false;
        
        if (currentPage is not null)
            await currentPage.OnLeaveAsync();

        if (result != StopInstancesAction.StopInstances)
            return true;
        
        var shutdownTasks = await _instanceService.RequestGracefulShutdownAllAsync();
        var shutdownProgressDialog = new ShutdownProgressDialogViewModel(shutdownTasks);
        await _overlayService.ShowAsync(shutdownProgressDialog);

        return true;
    }
}
