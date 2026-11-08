using D2RInstanceManager.Design.Services;
using AccountsPageViewModel = D2RInstanceManager.Account.ViewModels.AccountsPageViewModel;

namespace D2RInstanceManager.Design.ViewModels;

public class AccountsPageDesignViewModel : AccountsPageViewModel
{
    public AccountsPageDesignViewModel() : base(DesignServices.AccountService, DesignServices.OverlayService)
    {
        Refresh();
    }
}
