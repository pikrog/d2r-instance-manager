using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Account;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.GlobalSettings.Issues;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Page;
using AvaloniaApplication1.Region;
using AvaloniaApplication1.Selectable;
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
    
    private readonly GlobalSettingsValidator _globalSettingsValidator;
    
    private readonly OverlayService _overlayService;

    public ObservableCollection<GameInstanceItemViewModel> Instances { get; } = [];

    public bool IsTableEmpty => Instances.Count == 0;
    
    public SelectedItemsCollection<GameInstanceItemViewModel> SelectedInstances { get; }
    
    [ObservableProperty]
    public partial GlobalSettingsIssue? FirstGlobalSettingsIssue { get; set; }

    public InstancesPageViewModel(GameInstanceService gameInstanceService,
        AccountService accountService,
        RegionService regionService,
        DisplayService displayService,
        GlobalSettingsValidator globalSettingsValidator,
        OverlayService overlayService)
    {
        _gameInstanceService = gameInstanceService;
        _accountService = accountService;
        _regionService = regionService;
        _displayService = displayService;
        _globalSettingsValidator = globalSettingsValidator;
        _overlayService = overlayService;

        SelectedInstances = new SelectedItemsCollection<GameInstanceItemViewModel>(Instances);
        
        _gameInstanceService.InstanceStateChanged += OnInstanceStateChanged;
        SelectedInstances.CollectionChanged += OnSelectedInstancesChanged;
        SelectedInstances.ItemPropertyChanged += OnSelectedInstancePropertyChanged;
    }

    private void NotifyCanExecuteChangedForSelectedCommands()
    {
        LaunchSelectedCommand.NotifyCanExecuteChanged();
        StopSelectedCommand.NotifyCanExecuteChanged();
    }

    private void OnSelectedInstancesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        NotifyCanExecuteChangedForSelectedCommands();
    }

    private void OnSelectedInstancePropertyChanged(object? sender, ItemPropertyChangedEventArgs<GameInstanceItemViewModel> e)
    {
        NotifyCanExecuteChangedForSelectedCommands();
    }

    private void OnInstanceStateChanged(Guid id)
    {
        Dispatcher.UIThread.Post(() => RefreshItem(id));
    }

    private void RefreshItem(Guid id)
    {
        var (_, item) = Instances.Index().SingleOrDefault(r => r.Item.Id == id);
        if (item is null)
        {
            RefreshInstances();
            return;
        }
        
        var summary = _gameInstanceService.GetSummary(id);
        item.Id = summary.Id;
        item.Name = summary.Name;
        item.Status = summary.Status;
        item.IsActive = summary.IsActive;
        item.Issues.Clear();
        item.Issues.AddRange(summary.Issues);
    }

    private void RefreshInstances()
    {
        Instances.Clear();
        var instances = _gameInstanceService.GetSummaries()
            .Select(s => new GameInstanceItemViewModel(s.Id, s.Name, s.Status, s.IsActive, s.Issues));
        Instances.AddRange(instances);
    }

    private void RefreshGlobalSettingsIssues()
    {
        var globalSettingsIssues = _globalSettingsValidator.Validate();
        FirstGlobalSettingsIssue = globalSettingsIssues.FirstOrDefault(i => i.RequiresAttention);
    }

    private void Refresh()
    {
        RefreshInstances();
        RefreshGlobalSettingsIssues();
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
        RefreshInstances();
    }
    
    [RelayCommand]
    private async Task New()
    {
        await CoreEdit();
    }
    
    [RelayCommand]
    private async Task Edit(GameInstanceItemViewModel instance)
    {
        if (instance.IsActive)
            return;
        
        var snapshot = _gameInstanceService.GetInstanceConfigSnapshot(instance.Id);
        await CoreEdit(snapshot);
    }
    
    [RelayCommand]
    private async Task Delete(GameInstanceItemViewModel instance)
    {
        if (instance.IsActive)
            return;
        
        var confirmationViewModel = new DeleteInstanceDialogViewModel(instance.Name);
        var result = await _overlayService.ShowAsync(confirmationViewModel);
        if (!result)
            return;
        await _gameInstanceService.RemoveAsync(instance.Id);
        RefreshInstances();
    }
    
    private static bool CanLaunch(GameInstanceItemViewModel instance) => 
        instance is { RequiresAttention: false, IsActive: false };

    [RelayCommand]
    private async Task Launch(GameInstanceItemViewModel instance)
    {
        if (!CanLaunch(instance))
            return;
        
        await _gameInstanceService.LaunchAsync(instance.Id);
    }
    
    private static bool CanStop(GameInstanceItemViewModel instance) => instance.CanBeStopped;

    [RelayCommand]
    private async Task Stop(GameInstanceItemViewModel instance)
    {
        if (!CanStop(instance))
            return;
        
        await _gameInstanceService.StopAsync(instance.Id);
    }

    [RelayCommand]
    private void Show(GameInstanceItemViewModel instance)
    {
        if (!instance.IsActive)
            return;
        
        _gameInstanceService.Show(instance.Id);
    }
    
    private bool CanLaunchSelected => SelectedInstances.Items.Any(CanLaunch);
    
    [RelayCommand(CanExecute = nameof(CanLaunchSelected))]
    private async Task LaunchSelected()
    {
        var launchTasks = SelectedInstances.Items
            .Where(CanLaunch)
            .Select(i => _gameInstanceService.LaunchAsync(i.Id));
        await Task.WhenAll(launchTasks);
    }
    
    private bool CanStopSelected => SelectedInstances.Items.Any(CanStop);

    [RelayCommand(CanExecute = nameof(CanStopSelected))]
    private async Task StopSelected()
    {
        var stopTasks = SelectedInstances.Items
            .Where(CanStop)
            .Select(i => _gameInstanceService.StopAsync(i.Id));
        await Task.WhenAll(stopTasks);
    }
    
    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var instance in Instances)
            instance.IsSelected = false;
    }

    public override Task OnEnterAsync()
    {
        Refresh();
        return Task.CompletedTask;
    }
}
