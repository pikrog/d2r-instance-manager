using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels.Card;
using AvaloniaApplication1.ViewModels.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels.Account;

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
    
    public void Refresh()
    {
        var accounts = _accountService.GetAllSnapshots().ToList();
        
        var i = 0;
        for (; i < Math.Min(accounts.Count, _accounts.Count); i++)
        {
            var accountCard = _accounts[i];
            accountCard.Username = accounts[i].Username;
            accountCard.Password = accounts[i].Password;
            accountCard.Id = accounts[i].Id;
        }
        
        for(; i < accounts.Count; i++)
            _accounts.Add(new AccountCardViewModel(accounts[i].Id, accounts[i].Username, accounts[i].Password));
        
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
            _accounts.Remove(EditedCard);
        EditedCard = null;
    }

    public override void OnEnter() => Refresh();

    public override bool OnLeave() => true;
}
