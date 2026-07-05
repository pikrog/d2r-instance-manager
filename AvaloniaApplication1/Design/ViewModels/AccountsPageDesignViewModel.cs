using AvaloniaApplication1.Design.Services;
using AccountsPageViewModel = AvaloniaApplication1.ViewModels.Account.AccountsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class AccountsPageDesignViewModel : AvaloniaApplication1.ViewModels.Account.AccountsPageViewModel
{
    public AccountsPageDesignViewModel() : base(DesignServices.AccountService)
    {
        Refresh();
    }
}
