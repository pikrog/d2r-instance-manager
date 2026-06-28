using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Input;
using AvaloniaApplication1.Attributes;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Form;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels.Instance;

public partial class EditInstanceFormViewModel : FormViewModelBase
{
    public Guid? Id { get; set; }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsOnlineMode { get; set; }
    
    //public ObservableCollection<AccountTableRow> Accounts { get; set; } = [];
    public IReadOnlyList<AccountOption> Accounts { get; set; }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(IsOnlineMode), true)]
    public partial AccountOption? SelectedAccount { get; set; }

    public static IReadOnlyList<CredentialsVectorOption> CredentialsVectorOptions { get; } =
    [
        new(CredentialsVector.OsiTokenRegistry, "OSI Token registry"),
        new(CredentialsVector.CommandLineArguments, "Command-line arguments")
    ];

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredIf(nameof(IsOnlineMode), true)]
    public partial CredentialsVectorOption? SelectedCredentialsVector { get; set; } // = CredentialsVectorOptions.First();

    //public ObservableCollection<RegionTableRow> Regions { get; set; } = [];
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

    public HotKey RecallHotKey
    {
        get => new(RecallHotKeyGesture.Key, RecallHotKeyGesture.KeyModifiers);
        set => RecallHotKeyGesture = new KeyGesture(value.Key, value.KeyModifiers);
    }

    [ObservableProperty]
    public partial KeyGesture RecallHotKeyGesture { get; set; } = new(Key.None);

    [ObservableProperty]
    public partial bool IsNoSound { get; set; }

    [ObservableProperty]
    public partial bool IsWindowedMode { get; set; }

    public EditInstanceFormViewModel(IReadOnlyList<AccountOption> accounts, IReadOnlyList<RegionOption> regions, IReadOnlyList<DisplayOption> displays)
    {
        Accounts = accounts;
        Regions = regions;
        Displays = displays;
        
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IsOnlineMode) || IsOnlineMode)
            return;
        ClearErrors(nameof(SelectedAccount));
        ClearErrors(nameof(SelectedCredentialsVector));
        ClearErrors(nameof(SelectedRegion));
    }
}

