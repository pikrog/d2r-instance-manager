using AvaloniaApplication1.Design.Services;
using MainWindowViewModel = AvaloniaApplication1.MainWindow.MainWindowViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class MainWindowDesignViewModel() : MainWindowViewModel(DesignServices.OverlayHost,
    new InstancesPageDesignViewModel(),
    new AccountsPageDesignViewModel(),
    new RegionsPageDesignViewModel(),
    new GlobalSettingsPageDesignViewModel(),
    DesignServices.OverlayService,
    DesignServices.InstanceService);
