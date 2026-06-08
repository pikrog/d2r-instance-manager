using System.Collections.ObjectModel;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.Snapshots;
using DynamicData;

namespace AvaloniaApplication1.ViewModels;

public class AccountsPageViewModel : ViewModelBase
{
    private readonly AccountService _accountService;
    
    public ObservableCollection<AccountSnapshot> Accounts { get; } = [];

    public AccountsPageViewModel(AccountService accountService)
    {
        _accountService = accountService;
        UpdateAccounts();
    }

    private void UpdateAccounts()
    {
        Accounts.Clear();

        var accounts = _accountService.GetAllSnapshots();
        Accounts.AddRange(accounts);
    }
}
