using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using GlobalSettingsPageViewModel = AvaloniaApplication1.ViewModels.GlobalSettingsPageViewModel;

namespace AvaloniaApplication1.Views;

public partial class GlobalSettingsPageView : UserControl
{
    public GlobalSettingsPageView()
    {
        InitializeComponent();
        
        // todo: save on page change?
        AddHandler(LostFocusEvent, Internal_OnLostFocus, RoutingStrategies.Bubble);
        AddHandler(Button.ClickEvent, OnButtonClick, RoutingStrategies.Bubble);
    }

    private async Task SaveAsync()
    {
        if (DataContext is GlobalSettingsPageViewModel viewModel)
        {
            await viewModel.SaveAsync();
        }
    }

    private async void Internal_OnLostFocus(object? sender, FocusChangedEventArgs e) // todo: saving info, exceptions with async void, disable ui with overlay...?
    {
        await SaveAsync();
    }
    
    private async void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        await SaveAsync();
    }
}