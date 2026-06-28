using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Account;
using AccountsPageViewModel = AvaloniaApplication1.ViewModels.AccountsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class AccountsPageDesignViewModel() : AccountsPageViewModel(DesignServices.AccountService);
