using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.ViewModels.MainWindow;

namespace AvaloniaApplication1.Design.ViewModels;

public class MainWindowDesignViewModel : MainWindowViewModel
{
    public MainWindowDesignViewModel()
        : base(
            DesignServices.OverlayHost,
            new InstancesPageDesignViewModel(),
            new AccountsPageDesignViewModel(),
            new RegionsPageDesignViewModel(),
            new GlobalSettingsPageDesignViewModel())
    {
        
    }
}
