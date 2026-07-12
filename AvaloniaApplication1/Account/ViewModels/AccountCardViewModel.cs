using System;
using AvaloniaApplication1.Card.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Account.ViewModels;

public partial class AccountCardViewModel : CardViewModel
{
    public AccountCardViewModel(Guid? id, string? displayName, string username, string password, int instanceCount)
    {
        Id = id;
        DisplayName = displayName;
        Username = username;
        Password = password;
        InstanceCount = instanceCount;
    }

    public AccountCardViewModel()
    {
        Username = string.Empty;
        Password = string.Empty;
    }

    [ObservableProperty]
    public partial EditAccountFormViewModel? EditForm { get; private set; }
    
    public Guid? Id { get; set; }
    
    [ObservableProperty]
    public partial string? DisplayName { get; set; }
    
    [ObservableProperty]
    public partial string Username { get; set; }
    
    [ObservableProperty]
    public partial string Password { get; set; }
    
    [ObservableProperty]
    public partial int InstanceCount { get; set; }

    public void OpenForm()
    {
        EditForm = new EditAccountFormViewModel
        {
            Id = Id,
            DisplayName = DisplayName,
            Username = Username,
            Password = Password
        };
    }
    
    public void CloseForm()
    {
        EditForm = null;
    }
}