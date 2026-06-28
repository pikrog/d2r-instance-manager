using AvaloniaApplication1.ViewModels;
using MainWindowViewModel = AvaloniaApplication1.ViewModels.MainWindowViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class MainWindowDesignViewModel : AvaloniaApplication1.ViewModels.MainWindowViewModel
{
    public MainWindowDesignViewModel()
        : base(
            new InstancesPageDesignViewModel(),
            new AccountsPageDesignViewModel(),
            new RegionsPageDesignViewModel(),
            new GlobalSettingsPageDesignViewModel())
    {
    }
}
