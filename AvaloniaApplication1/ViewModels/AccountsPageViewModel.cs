using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels.Account;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class AccountsPageViewModel : ViewModelBase
{
    private readonly AccountService _accountService;

    public ObservableCollection<AccountCardViewModel> Accounts { get; } = [];

    public bool IsNotEditing => EditedCard is null;
    
    [NotifyCanExecuteChangedFor(nameof(NewCommand), nameof(EditCommand), nameof(DeleteCommand))]
    [ObservableProperty]
    public partial AccountCardViewModel? EditedCard { get; set; }

    public AccountsPageViewModel(AccountService accountService)
    {
        _accountService = accountService;
        Refresh();
    }
    
    public void Refresh()
    {
        // todo: rely on service event instead of polling
        
        var accounts = _accountService.GetAllSnapshots().ToList();
        
        var i = 0;
        for (; i < Math.Min(accounts.Count, Accounts.Count); i++)
        {
            Accounts[i].Id = accounts[i].Id;
            Accounts[i].Username = accounts[i].Username;
            Accounts[i].Password = accounts[i].Password;
        }
        
        for(; i < accounts.Count; i++)
            Accounts.Add(new AccountCardViewModel(accounts[i].Id, accounts[i].Username, accounts[i].Password));
        
        for (; i < Accounts.Count; i++)
            Accounts.RemoveAt(i);
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void New()
    {
        var account = new AccountCardViewModel();
        Accounts.Add(account);
        Edit(account);
    }

    [RelayCommand]
    private async Task Save(EditAccountFormViewModel form)
    {
        if (!form.Validate())
            return;
        
        var draft = new AccountDraft(form.Id, form.Username, form.Password);
        await _accountService.SaveAsync(draft);
        
        EndEdit();
        Refresh();
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void Edit(AccountCardViewModel account)
    {
        EndEdit();
        EditedCard = account;
        EditedCard.OpenForm();
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private async Task Delete(AccountCardViewModel account)
    {
        EndEdit();
        await _accountService.RemoveAsync(account.Id!.Value);
        Refresh();
    }

    [RelayCommand]
    private void EndEdit()
    {
        EditedCard?.CloseForm();
        if (EditedCard is not null && EditedCard.Id is null)
            Accounts.Remove(EditedCard);
        EditedCard = null;
    }
}
