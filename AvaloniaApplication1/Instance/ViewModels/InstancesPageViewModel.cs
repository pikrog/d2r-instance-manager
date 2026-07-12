using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Account;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Page;
using AvaloniaApplication1.Region;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;

namespace AvaloniaApplication1.Instance.ViewModels;

public partial class InstancesPageViewModel : PageViewModel, IDialogParticipant
{
    private readonly GameInstanceService _gameInstanceService;

    private readonly AccountService _accountService;
    
    private readonly RegionService _regionService;

    private readonly DisplayService _displayService;
    
    private readonly OverlayService _overlayService;

    public ObservableCollection<GameInstanceTableRow> Instances { get; } = [];
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand), nameof(DeleteCommand), nameof(LaunchCommand))]
    public partial GameInstanceTableRow? SelectedInstance { get; set; }

    public ObservableCollection<GameInstanceTableRow> SelectedInstances { get; set; } = [];

    [MemberNotNullWhen(true, nameof(SelectedInstance))] // todo? remove
    public bool IsInstanceSelected => SelectedInstance is not null;

    public bool IsTableEmpty => Instances.Count == 0;

    public InstancesPageViewModel(GameInstanceService gameInstanceService,
        AccountService accountService,
        RegionService regionService,
        DisplayService displayService,
        OverlayService overlayService)
    {
        _gameInstanceService = gameInstanceService;
        _accountService = accountService;
        _regionService = regionService;
        _displayService = displayService;
        _overlayService = overlayService;

        _gameInstanceService.InstanceStateChanged += OnInstanceStateChanged;
    }

    private void OnInstanceStateChanged(Guid id)
    {
        Dispatcher.UIThread.Post(() => RefreshRow(id));
    }

    private void RefreshRow(Guid id)
    {
        var (targetRowId, targetRow) = Instances.Index().SingleOrDefault(r => r.Item.Id == id);
        if (targetRow is null)
        {
            Refresh();
            return;
        }
        
        var newRow = _gameInstanceService.GetTableRow(id);
        Instances[targetRowId] = newRow;
    }

    private void Refresh()
    {
        Instances.Clear();
        var table = _gameInstanceService.GetTable();
        Instances.AddRange(table);
    }

    private async Task<EditInstanceFormViewModel> CreateForm(GameInstanceSnapshot? snapshot = null)
    {
        var accountOptions = _accountService.GetOptions();
        var regionOptions = _regionService.GetOptions();
        var displayOptions = await _displayService.GetOptionsAsync(snapshot?.Display as DisplaySelection.Specific);
        var form = new EditInstanceFormViewModel(accountOptions, regionOptions, displayOptions)
        {
            SelectedDisplay = displayOptions[0],
        };

        if (snapshot is null)
            return form;
        
        var selectedAccount = accountOptions.SingleOrDefault(x => x.Id == snapshot.AccountId);
        var selectedRegion = regionOptions.SingleOrDefault(x => x.Id == snapshot.RegionId);
        var selectedAuthenticationMethod = EditInstanceFormViewModel.AuthenticationMethodOptions.SingleOrDefault(o => o.AuthenticationMethod == snapshot.AuthenticationMethod);
        var selectedDisplay = displayOptions.SingleOrDefault(o => DisplayMatcher.IsMatch(snapshot.Display, o)) ?? displayOptions[0];
        
        form.Id = snapshot.Id;
        form.Name = snapshot.Name;
        form.IsOnlineMode = snapshot.IsOnlineMode;
        form.SelectedAccount = selectedAccount;
        form.SelectedAuthenticationMethod = selectedAuthenticationMethod;
        form.SelectedRegion = selectedRegion;
        form.SelectedDisplay = selectedDisplay;
        form.RecallHotKey = snapshot.RecallHotKey;
        form.IsNoSound = snapshot.IsNoSound;
        form.IsWindowedMode = snapshot.IsWindowedMode;
        return form;
    }

    private GameInstanceDraft CreateDraft(EditInstanceFormViewModel form)
    {
        return new GameInstanceDraft(
            form.Id,
            form.Name,
            form.IsOnlineMode,
            form.SelectedAccount?.Id,
            form.SelectedAuthenticationMethod?.AuthenticationMethod,
            form.SelectedRegion?.Id,
            DisplaySelectionMapper.Map(form.SelectedDisplay),
            form.IsNoSound,
            form.IsWindowedMode, 
            form.RecallHotKey
        );
    }
    
    private async Task CoreEdit(GameInstanceSnapshot? snapshot = null)
    {
        var form = await CreateForm(snapshot);
        var okPressed = await this.OpenForm(form);
        if (!okPressed)
            return;
        var draft = CreateDraft(form);
        await _gameInstanceService.SaveAsync(draft);
        Refresh();
    }
    
    [RelayCommand]
    private async Task New()
    {
        await CoreEdit();
    }
    
    [RelayCommand]
    private async Task Edit(GameInstanceTableRow instance)
    {
        var snapshot = _gameInstanceService.GetInstanceConfigSnapshot(instance.Id);
        await CoreEdit(snapshot);
    }
    
    [RelayCommand]
    private async Task Delete(GameInstanceTableRow instance)
    {
        var confirmationViewModel = new DeleteInstanceDialogViewModel(instance.Name);
        var result = await _overlayService.ShowAsync(confirmationViewModel);
        if (!result)
            return;
        await _gameInstanceService.RemoveAsync(instance.Id);
        Refresh();
    }

    [RelayCommand]
    private async Task Launch(GameInstanceTableRow instance)
    {
        await _gameInstanceService.LaunchAsync(instance.Id);
    }

    [RelayCommand]
    private async Task Stop(GameInstanceTableRow instance)
    {
        await _gameInstanceService.StopAsync(instance.Id);
    }

    [RelayCommand]
    private void Show(GameInstanceTableRow instance)
    {
        _gameInstanceService.Show(instance.Id);
    }

    public override void OnEnter() => Refresh();

    public override bool OnLeave() => true;
}
