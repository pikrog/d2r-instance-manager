using System;
using AvaloniaApplication1.ViewModels.Card;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels.Account;

public partial class AccountCardViewModel(Guid? id = null, string username = "", string password = "") : CardViewModel
{
    [ObservableProperty]
    public partial EditAccountFormViewModel? EditForm { get; private set; }
    
    public Guid? Id { get; set; } = id;
    
    [ObservableProperty]
    public partial string Username { get; set; } = username;
    
    [ObservableProperty]
    public partial string Password { get; set; } = password;

    public void OpenForm()
    {
        EditForm = new EditAccountFormViewModel
        {
            Id = Id,
            Username = Username,
            Password = Password
        };
    }
    
    public void CloseForm()
    {
        EditForm = null;
    }
}