using D2RInstanceManager.Design.Services;
using MainWindowViewModel = D2RInstanceManager.MainWindow.MainWindowViewModel;

namespace D2RInstanceManager.Design.ViewModels;

public class MainWindowDesignViewModel : MainWindowViewModel
{
    public MainWindowDesignViewModel() : base(DesignServices.PageHost,
        DesignServices.NavigationService,
        DesignServices.OverlayHost,
        DesignServices.OverlayService,
        DesignServices.ToastHost,
        DesignServices.InstanceService,
        DesignServices.InstanceNotificationCounter,
        DesignServices.LogNotificationCounter)
    {
        SetupAsync().GetAwaiter().GetResult();
    }
}
