using AvaloniaApplication1.ViewModels.Common;
using AvaloniaApplication1.ViewModels.Common.Dialog;
using AvaloniaApplication1.ViewModels.Common.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels.MainWindow;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty]
    public partial PageViewModel CurrentPage { get; set; }

    public Instance.InstancesPageViewModel InstancesPageViewModel { get; }

    public Account.AccountsPageViewModel AccountsPageViewModel { get; }

    public Region.RegionsPageViewModel RegionsPageViewModel { get; }
    
    public GlobalSettings.GlobalSettingsPageViewModel GlobalSettingsPageViewModel { get; }

    public MainWindowViewModel(
        Instance.InstancesPageViewModel instancesPageViewModel,
        Account.AccountsPageViewModel accountsPageViewModel,
        Region.RegionsPageViewModel regionsPageViewModel,
        GlobalSettings.GlobalSettingsPageViewModel globalSettingsPageViewModel)
    {
        InstancesPageViewModel = instancesPageViewModel;
        AccountsPageViewModel = accountsPageViewModel;
        RegionsPageViewModel = regionsPageViewModel;
        GlobalSettingsPageViewModel = globalSettingsPageViewModel;

        CurrentPage = InstancesPageViewModel;
        CurrentPage.OnEnter();
    }

    [RelayCommand]
    private void SetPage(PageViewModel page)
    {
        if (!CurrentPage.OnLeave())
            return;
        CurrentPage = page;
        CurrentPage.OnEnter();
    }
}
