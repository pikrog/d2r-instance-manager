using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public class AccountsPageDesignViewModel() : AccountsPageViewModel(DesignServices.AccountService);
