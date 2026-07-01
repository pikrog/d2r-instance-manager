using AvaloniaApplication1.ViewModels.Dialog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty]
    public partial PageViewModel CurrentPage { get; set; }

    public InstancesPageViewModel InstancesPageViewModel { get; }

    public AccountsPageViewModel AccountsPageViewModel { get; }

    public RegionsPageViewModel RegionsPageViewModel { get; }
    
    public GlobalSettingsPageViewModel GlobalSettingsPageViewModel { get; }

    public MainWindowViewModel(
        InstancesPageViewModel instancesPageViewModel,
        AccountsPageViewModel accountsPageViewModel,
        RegionsPageViewModel regionsPageViewModel,
        GlobalSettingsPageViewModel globalSettingsPageViewModel)
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
