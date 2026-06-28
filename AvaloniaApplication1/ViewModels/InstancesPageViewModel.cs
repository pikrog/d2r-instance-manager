using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Mappers;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.Snapshots;
using AvaloniaApplication1.ViewModels.Dialog;
using AvaloniaApplication1.ViewModels.Instance;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;

namespace AvaloniaApplication1.ViewModels;

public partial class InstancesPageViewModel : ViewModelBase, IDialogParticipant
{
    private readonly GameInstanceService _gameInstanceService;

    private readonly AccountService _accountService;
    
    private readonly RegionService _regionService;

    private readonly DisplayService _displayService;

    public ObservableCollection<GameInstanceTableRow> Instances { get; } = [];
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand), nameof(RemoveCommand), nameof(LaunchCommand))]
    public partial GameInstanceTableRow? SelectedInstance { get; set; }

    public ObservableCollection<GameInstanceTableRow> SelectedInstances { get; set; } = [];

    [MemberNotNullWhen(true, nameof(SelectedInstance))] // todo? remove
    public bool IsInstanceSelected => SelectedInstance is not null;
    
    public InstancesPageViewModel(GameInstanceService gameInstanceService,
        AccountService accountService,
        RegionService regionService,
        DisplayService displayService)
    {
        _gameInstanceService = gameInstanceService;
        _accountService = accountService;
        _regionService = regionService;
        _displayService = displayService;

        _gameInstanceService.InstanceStateChanged += OnInstanceStateChanged;
        
        RefreshTable();
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
            RefreshTable();
            return;
        }
        
        var newRow = _gameInstanceService.GetTableRow(id);
        Instances[targetRowId] = newRow;
    }

    private void RefreshTable()
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
        var selectedCredentialsVector = EditInstanceFormViewModel.CredentialsVectorOptions.SingleOrDefault(o => o.CredentialsVector == snapshot.CredentialsVector);
        var selectedDisplay = displayOptions.SingleOrDefault(o => DisplayMatcher.IsMatch(snapshot.Display, o)) ?? displayOptions[0];
        
        form.Id = snapshot.Id;
        form.Name = snapshot.Name;
        form.IsOnlineMode = snapshot.IsOnlineMode;
        form.SelectedAccount = selectedAccount;
        form.SelectedCredentialsVector = selectedCredentialsVector;
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
            form.SelectedCredentialsVector?.CredentialsVector,
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
        RefreshTable();
    }
    
    [RelayCommand]
    private async Task New()
    {
        await CoreEdit();
    }
    
    [RelayCommand(CanExecute = nameof(IsInstanceSelected))]
    private async Task Edit() // todo: pass instance or no args and just get selected item?
    {
        if (SelectedInstance is null)
            return;
        var snapshot = _gameInstanceService.GetInstanceConfigSnapshot(SelectedInstance.Id);
        await CoreEdit(snapshot);
    }
    
    [RelayCommand(CanExecute = nameof(IsInstanceSelected))]
    private async Task Remove()
    {
        if (SelectedInstance is null)
            return;
        await _gameInstanceService.RemoveAsync(SelectedInstance.Id);
        RefreshTable();
    }

    [RelayCommand(CanExecute = nameof(IsInstanceSelected))]
    private async Task Launch()
    {
        if (SelectedInstances.Count == 0)
            return;
        foreach (var instance in SelectedInstances)
            await _gameInstanceService.LaunchAsync(instance.Id);
    }
}
