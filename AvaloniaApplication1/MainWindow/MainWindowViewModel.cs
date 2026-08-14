using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Overlay.Dialog.ShutdownProgress;
using AvaloniaApplication1.Overlay.Dialog.StopInstances;
using AvaloniaApplication1.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AccountsPageViewModel = AvaloniaApplication1.Account.ViewModels.AccountsPageViewModel;
using GlobalSettingsPageViewModel = AvaloniaApplication1.GlobalSettings.GlobalSettingsPageViewModel;
using OverlayHost = AvaloniaApplication1.Overlay.OverlayHost;
using RegionsPageViewModel = AvaloniaApplication1.Region.ViewModels.RegionsPageViewModel;

namespace AvaloniaApplication1.MainWindow;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty]
    public partial PageViewModel CurrentPage { get; set; }

    public InstancesPageViewModel InstancesPageViewModel { get; }

    public AccountsPageViewModel AccountsPageViewModel { get; }

    public RegionsPageViewModel RegionsPageViewModel { get; }
    
    public GlobalSettingsPageViewModel GlobalSettingsPageViewModel { get; }

    public OverlayHost OverlayHost { get; }
    
    public OverlayService OverlayService { get; }
    
    public InstanceService InstanceService { get; }

    public MainWindowViewModel(
        OverlayHost overlayHost,
        InstancesPageViewModel instancesPageViewModel,
        AccountsPageViewModel accountsPageViewModel,
        RegionsPageViewModel regionsPageViewModel,
        GlobalSettingsPageViewModel globalSettingsPageViewModel,
        OverlayService overlayService,
        InstanceService instanceService)
    {
        InstancesPageViewModel = instancesPageViewModel;
        AccountsPageViewModel = accountsPageViewModel;
        RegionsPageViewModel = regionsPageViewModel;
        GlobalSettingsPageViewModel = globalSettingsPageViewModel;
        OverlayHost = overlayHost;
        OverlayService = overlayService;
        InstanceService = instanceService;

        CurrentPage = InstancesPageViewModel;
    }

    public async Task SetupAsync()
    {
        await CurrentPage.OnEnterAsync();
    }

    [RelayCommand]
    private async Task SetPage(PageViewModel page)
    {
        if (CurrentPage == page)
            return;
        if (!await CurrentPage.CanLeaveAsync())
            return;
        await CurrentPage.OnLeaveAsync();
        CurrentPage = page;
        await CurrentPage.OnEnterAsync();
    }
    
    public async Task<bool> TryExitAsync()
    {
        if (!await CurrentPage.CanLeaveAsync())
            return false;

        var activeInstanceCount = await InstanceService.GetActiveCountAsync();
        if (activeInstanceCount == 0)
        {
            await CurrentPage.OnLeaveAsync();
            return true;
        }
        
        var stopInstancesDialog = new StopInstancesDialogViewModel(activeInstanceCount);
        var result = await OverlayService.ShowAsync(stopInstancesDialog);
        if (result == StopInstancesAction.Cancel)
            return false;
        
        await CurrentPage.OnLeaveAsync();

        if (result != StopInstancesAction.StopInstances)
            return true;
        
        var shutdownTasks = await InstanceService.RequestGracefulShutdownAllAsync();
        var shutdownProgressDialog = new ShutdownProgressDialogViewModel(shutdownTasks);
        await OverlayService.ShowAsync(shutdownProgressDialog);

        return true;
    }
}
