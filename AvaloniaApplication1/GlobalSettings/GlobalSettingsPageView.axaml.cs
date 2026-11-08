using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaApplication1.GlobalSettings;

public partial class GlobalSettingsPageView : UserControl
{
    public GlobalSettingsPageView()
    {
        InitializeComponent();
    }

    private void GameExecutablePathTextBox_LostFocus(object? sender, FocusChangedEventArgs e)
    {
        if (DataContext is not GlobalSettingsPageViewModel viewModel)
            return;
        
        viewModel.ValidateGameExecutablePath();
    }
}