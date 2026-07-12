using AvaloniaApplication1.Design.Services;
using AccountsPageViewModel = AvaloniaApplication1.Account.ViewModels.AccountsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class AccountsPageDesignViewModel : AccountsPageViewModel
{
    public AccountsPageDesignViewModel() : base(DesignServices.AccountService, DesignServices.OverlayService)
    {
        Refresh();
    }
}
