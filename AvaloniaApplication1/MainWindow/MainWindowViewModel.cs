using System;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Overlay.Dialog.ShutdownProgress;
using AvaloniaApplication1.Overlay.Dialog.StopInstances;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.MainWindow;

public partial class MainWindowViewModel(
    IPageHost pageHost,
    NavigationService navigationService,
    IOverlayHost overlayHost,
    OverlayService overlayService,
    InstanceService instanceService)
    : ViewModelBase, IDialogParticipant
{

    public IPageHost PageHost { get; } = pageHost;

    public IOverlayHost OverlayHost { get; } = overlayHost;

    public async Task SetupAsync() => await navigationService.NavigateAsync<InstancesPageViewModel>();
    
    [RelayCommand]
    private async Task SetPage(Type pageType) => await navigationService.NavigateAsync(pageType);
    
    public async Task<bool> TryExitAsync()
    {
        var currentPage = PageHost.CurrentPage;
        if (currentPage is not null && !await currentPage.CanLeaveAsync())
            return false;

        var activeInstanceCount = await instanceService.GetActiveCountAsync();
        if (activeInstanceCount == 0)
        {
            if (currentPage is not null)
                await currentPage.OnLeaveAsync();
            return true;
        }
        
        var stopInstancesDialog = new StopInstancesDialogViewModel(activeInstanceCount);
        var result = await overlayService.ShowAsync(stopInstancesDialog);
        if (result == StopInstancesAction.Cancel)
            return false;
        
        if (currentPage is not null)
            await currentPage.OnLeaveAsync();

        if (result != StopInstancesAction.StopInstances)
            return true;
        
        var shutdownTasks = await instanceService.RequestGracefulShutdownAllAsync();
        var shutdownProgressDialog = new ShutdownProgressDialogViewModel(shutdownTasks);
        await overlayService.ShowAsync(shutdownProgressDialog);

        return true;
    }
}
