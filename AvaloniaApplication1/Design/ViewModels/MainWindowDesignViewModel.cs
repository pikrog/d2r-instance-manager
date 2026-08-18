using AvaloniaApplication1.Design.Services;
using MainWindowViewModel = AvaloniaApplication1.MainWindow.MainWindowViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class MainWindowDesignViewModel : MainWindowViewModel
{
    public MainWindowDesignViewModel() : base(DesignServices.PageHost,
        DesignServices.NavigationService,
        DesignServices.OverlayHost,
        DesignServices.OverlayService,
        DesignServices.InstanceService)
    {
        SetupAsync().GetAwaiter().GetResult();
    }
}
