using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Avalonia.Input;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Authentication.Models;
using D2RInstanceManager.Common;
using D2RInstanceManager.Display;
using D2RInstanceManager.Form.ViewModels;
using D2RInstanceManager.HotKey;
using D2RInstanceManager.HotKey.Config;
using D2RInstanceManager.HotKey.Coordination;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Region.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Instance.ViewModels;

public partial class EditInstanceFormViewModel : FormViewModelBase
{
    public Guid? Id { get; set; }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsOnlineMode { get; set; }
    
    public IReadOnlyList<AccountOption> Accounts { get; set; }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(IsOnlineMode), true)]
    public partial AccountOption? SelectedAccount { get; set; }

    public static IReadOnlyList<AuthenticationMethodOption> AuthenticationMethodOptions { get; } =
    [
        new(AuthenticationMethod.OsiTokenRegistry, "OSI Token registry"),
        new(AuthenticationMethod.CommandLineArguments, "Command-line arguments")
    ];

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(IsOnlineMode), true)]
    public partial AuthenticationMethodOption? SelectedAuthenticationMethod { get; set; }
    
    public IReadOnlyList<RegionOption> Regions { get; set; }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(IsOnlineMode), true)]
    public partial RegionOption? SelectedRegion { get; set; }
    
    public IReadOnlyList<DisplayOption> Displays { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial DisplayOption SelectedDisplay { get; set; }

    public HotKeyCommand? ShowCommandHotKeyIdentity => Id.HasValue ? new ShowInstanceHotKeyCommand(Id.Value) : null;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotBoundHotKey(nameof(ShowCommandHotKeyIdentity))]
    public partial KeyCombination ShowCommandHotKey { get; set; } = new(Key.None);

    [ObservableProperty]
    public partial bool IsNoSound { get; set; }

    [ObservableProperty]
    public partial bool IsWindowedMode { get; set; }

    public EditInstanceFormViewModel(IHotKeyConfigValidator hotKeyConfigValidator,
        IReadOnlyList<AccountOption> accounts,
        IReadOnlyList<RegionOption> regions,
        IReadOnlyList<DisplayOption> displays)
        : base(new HotKeyConfigValidatorProvider(hotKeyConfigValidator))
    {
        Accounts = accounts;
        Regions = regions;
        Displays = displays;
    }

    partial void OnIsOnlineModeChanged(bool value)
    {
        if (value)
            return;
        
        ClearErrors(nameof(SelectedAccount));
        ClearErrors(nameof(SelectedAuthenticationMethod));
        ClearErrors(nameof(SelectedRegion));
    }
}

