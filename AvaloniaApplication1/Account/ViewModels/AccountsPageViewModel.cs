using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Card;
using AvaloniaApplication1.Card.ViewModels;
using AvaloniaApplication1.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Account.ViewModels;

public partial class AccountsPageViewModel : PageViewModel
{
    private readonly AccountService _accountService;

    private readonly ObservableCollection<AccountCardViewModel> _accounts = [];
    
    private readonly CardCollection<AccountCardViewModel> _cards;
    
    public ReadOnlyObservableCollection<CardViewModel> Cards => _cards.Cards;

    public bool IsNotEditing => EditedCard is null;
    
    [NotifyCanExecuteChangedFor(nameof(NewCommand), nameof(EditCommand), nameof(DeleteCommand))]
    [ObservableProperty]
    public partial AccountCardViewModel? EditedCard { get; set; }

    public AccountsPageViewModel(AccountService accountService)
    {
        _accountService = accountService;
        
        _cards = new CardCollection<AccountCardViewModel>(_accounts);
    }
    
    public void Refresh() // todo: event-driven updates? 1st: OnAccountsChanged from Service with event type Added/Updated/Removed
    {
        var accounts = _accountService.GetSummaries()
            .Select(s => new AccountCardViewModel(s.Id, s.DisplayName, s.Username, s.Password, s.InstanceCount))
            .ToList();
        
        var i = 0;
        for (; i < Math.Min(accounts.Count, _accounts.Count); i++)
        {
            var savedCard = _accounts[i];
            savedCard.Id = accounts[i].Id;
            savedCard.DisplayName = accounts[i].DisplayName;
            savedCard.Username = accounts[i].Username;
            savedCard.Password = accounts[i].Password;
            savedCard.InstanceCount = accounts[i].InstanceCount;
        }
        
        for(; i < accounts.Count; i++)
            _accounts.Add(accounts[i]);
        
        for (; i < _accounts.Count; )
            _accounts.RemoveAt(i);
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void New()
    {
        var account = new AccountCardViewModel();
        _accounts.Add(account);
        Edit(account);
    }

    [RelayCommand]
    private async Task Save(EditAccountFormViewModel form)
    {
        if (!form.Validate())
            return;
        
        var draft = new AccountDraft(form.Id, form.DisplayName, form.Username, form.Password);
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
            _accounts.Remove(EditedCard);
        EditedCard = null;
    }

    public override void OnEnter() => Refresh();

    public override bool OnLeave() => true;
}
