using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Views;

public partial class GlobalSettingsPageView : UserControl
{
    public GlobalSettingsPageView()
    {
        InitializeComponent();
        
        Panel.AddHandler(LostFocusEvent, Internal_OnLostFocus, RoutingStrategies.Bubble);
    }

    private async Task Save()
    {
        if (DataContext is GlobalSettingsPageViewModel viewModel)
        {
            await viewModel.Save();
        }
    }

    private async void Internal_OnLostFocus(object? sender, FocusChangedEventArgs e) // todo: saving info, exceptions with async void, disable ui with overlay...?
    {
        await Save();
    }
}