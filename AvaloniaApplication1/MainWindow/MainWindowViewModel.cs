using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Overlay.Dialog.ShutdownProgress;
using AvaloniaApplication1.Overlay.Dialog.StopInstances;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.MainWindow;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    public IPageHost PageHost { get; }

    public IOverlayHost OverlayHost { get; }
    
    private readonly NavigationService _navigationService;
    
    private readonly OverlayService _overlayService;
    
    private readonly InstanceService _instanceService;

    [ObservableProperty]
    public partial int InstancesNotificationCount { get; private set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSingleInstanceActive), nameof(AreMultipleInstancesActive))]
    public partial int ActiveInstancesCount { get; private set; }

    public bool IsSingleInstanceActive => ActiveInstancesCount == 1;
    
    public bool AreMultipleInstancesActive => ActiveInstancesCount > 1;
    
    public MainWindowViewModel(IPageHost pageHost,
        NavigationService navigationService,
        IOverlayHost overlayHost,
        OverlayService overlayService,
        InstanceService instanceService)
    {
        _navigationService = navigationService;
        _overlayService = overlayService;
        _instanceService = instanceService;
        
        PageHost = pageHost;
        OverlayHost = overlayHost;

        _instanceService.ActiveCountChanged += (_, _) => Dispatcher.UIThread.Post(UpdateActiveInstancesCount);
        _instanceService.NotificationCountChanged += (_, _) => Dispatcher.UIThread.Post(UpdateInstancesNotificationCount);
        
        UpdateActiveInstancesCount();
        UpdateInstancesNotificationCount();
    }

    private void UpdateActiveInstancesCount()
    {
        ActiveInstancesCount = _instanceService.CachedActiveCount;
    }

    private void UpdateInstancesNotificationCount()
    {
        InstancesNotificationCount = _instanceService.UnacknowledgedNotificationCount;
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
