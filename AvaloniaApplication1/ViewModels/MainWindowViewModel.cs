using AvaloniaApplication1.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public InstancesPageViewModel InstancesPageViewModel { get; }

    public AccountsPageViewModel AccountsPageViewModel { get; }

    public RegionsPageViewModel RegionsPageViewModel { get; }

    public MainWindowViewModel(
        InstancesPageViewModel instancesPageViewModel,
        AccountsPageViewModel accountsPageViewModel,
        RegionsPageViewModel regionsPageViewModel)
    {
        InstancesPageViewModel = instancesPageViewModel;
        AccountsPageViewModel = accountsPageViewModel;
        RegionsPageViewModel = regionsPageViewModel;

        CurrentPage = InstancesPageViewModel;
    }

    [RelayCommand]
    private void SetPage(ViewModelBase page)
    {
        CurrentPage = page;
    }
}
