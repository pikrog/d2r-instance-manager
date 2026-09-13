using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Avalonia.Input;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Form.ViewModels;
using AvaloniaApplication1.HotKey;
using AvaloniaApplication1.HotKey.Config;
using AvaloniaApplication1.HotKey.Coordination;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Instance.ViewModels;

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

