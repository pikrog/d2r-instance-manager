using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public class MainWindowDesignViewModel : MainWindowViewModel
{
    public MainWindowDesignViewModel()
        : base(
            new InstancesPageDesignViewModel(),
            new AccountsPageDesignViewModel(),
            new RegionsPageDesignViewModel())
    {
    }
}
