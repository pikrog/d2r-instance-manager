using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    private readonly InstanceService _instanceService;

    private readonly AccountService _accountService;
    
    private readonly RegionService _regionService;

    private readonly DisplayService _displayService;
    
    private readonly GlobalSettingsValidator _globalSettingsValidator;
    
    private readonly OverlayService _overlayService;

    public ObservableCollection<InstanceItemViewModel> Instances { get; } = [];

    public bool IsTableEmpty => Instances.Count == 0;
    
    public SelectedItemsCollection<InstanceItemViewModel> SelectedInstances { get; }
    
    [ObservableProperty]
    public partial GlobalSettingsIssue? FirstGlobalSettingsIssue { get; set; }

    public InstancesPageViewModel(InstanceService instanceService,
        AccountService accountService,
        RegionService regionService,
        DisplayService displayService,
        GlobalSettingsValidator globalSettingsValidator,
        OverlayService overlayService)
    {
        _instanceService = instanceService;
        _accountService = accountService;
        _regionService = regionService;
        _displayService = displayService;
        _globalSettingsValidator = globalSettingsValidator;
        _overlayService = overlayService;

        SelectedInstances = new SelectedItemsCollection<InstanceItemViewModel>(Instances);
        
        _instanceService.InstanceStateChanged += OnInstanceStateChanged;
        SelectedInstances.CollectionChanged += OnSelectedInstancesChanged;
    }
    
    private void OnSelectedInstancesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        LaunchSelectedCommand.NotifyCanExecuteChanged();
        StopSelectedCommand.NotifyCanExecuteChanged();
    }

    private void OnInstanceStateChanged(Guid id)
    {
        Dispatcher.UIThread.Post(() => RefreshItem(id));
    }

    private void OnInstancePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(InstanceItemViewModel.CanLaunch):
                LaunchCommand.NotifyCanExecuteChanged();
                LaunchSelectedCommand.NotifyCanExecuteChanged();
                break;
            case nameof(InstanceItemViewModel.CanStop):
                StopCommand.NotifyCanExecuteChanged();
                StopSelectedCommand.NotifyCanExecuteChanged();
                break;
            case nameof(InstanceItemViewModel.CanEdit):
                EditCommand.NotifyCanExecuteChanged();
                break;
            case nameof(InstanceItemViewModel.CanDelete):
                DeleteCommand.NotifyCanExecuteChanged();
                break;
            case nameof(InstanceItemViewModel.CanShow):
                ShowCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void RefreshItem(Guid id)
    {
        var (_, item) = Instances.Index().SingleOrDefault(r => r.Item.Id == id);
        if (item is null)
        {
            RefreshInstances();
            return;
        }
        
        var summary = _instanceService.GetSummary(id);
        item.Id = summary.Id;
        item.Name = summary.Name;
        item.Status = summary.Status;
        item.IsActive = summary.IsActive;
        item.Issues.Clear();
        item.Issues.AddRange(summary.Issues);
    }

    private void RefreshInstances()
    {
        foreach (var instance in Instances)
            instance.PropertyChanged -= OnInstancePropertyChanged;
        
        Instances.Clear();
        var instances = _instanceService.GetSummaries()
            .Select(s => new InstanceItemViewModel(s.Id, s.Name, s.Status, s.IsActive, s.Issues));
        Instances.AddRange(instances);
        
        foreach (var instance in Instances)
            instance.PropertyChanged += OnInstancePropertyChanged;
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

    private async Task<EditInstanceFormViewModel> CreateForm(InstanceSnapshot? snapshot = null)
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

    private InstanceDraft CreateDraft(EditInstanceFormViewModel form)
    {
        return new InstanceDraft(
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
    
    private async Task CoreEdit(InstanceSnapshot? snapshot = null)
    {
        var form = await CreateForm(snapshot);
        var okPressed = await this.OpenForm(form);
        if (!okPressed)
            return;
        var draft = CreateDraft(form);
        await _instanceService.SaveAsync(draft);
        RefreshInstances();
    }
    
    [RelayCommand]
    private async Task New()
    {
        await CoreEdit();
    }

    private bool CanEdit(InstanceItemViewModel instance) => instance.CanEdit;
    
    [RelayCommand(CanExecute = nameof(CanEdit))]
    private async Task Edit(InstanceItemViewModel instance)
    {
        if (!CanEdit(instance))
            return;
        
        var snapshot = _instanceService.GetInstanceConfigSnapshot(instance.Id);
        await CoreEdit(snapshot);
    }
    
    private bool CanDelete(InstanceItemViewModel instance) => instance.CanDelete;
    
    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task Delete(InstanceItemViewModel instance)
    {
        if (!CanDelete(instance))
            return;
        
        var confirmationViewModel = new DeleteInstanceDialogViewModel(instance.Name);
        var result = await _overlayService.ShowAsync(confirmationViewModel);
        if (!result)
            return;
        await _instanceService.RemoveAsync(instance.Id);
        RefreshInstances();
    }
    
    private static bool CanLaunch(InstanceItemViewModel instance) => instance.CanLaunch;

    [RelayCommand(CanExecute = nameof(CanLaunch))]
    private async Task Launch(InstanceItemViewModel instance)
    {
        if (!CanLaunch(instance))
            return;
        
        instance.IsLaunchPending = true;
        try
        {
            await _instanceService.LaunchAsync(instance.Id);
        }
        finally
        {
            instance.IsLaunchPending = false;
        }
    }
    
    private static bool CanStop(InstanceItemViewModel instance) => instance.CanStop;

    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task Stop(InstanceItemViewModel instance)
    {
        if (!CanStop(instance))
            return;
        
        await _instanceService.StopAsync(instance.Id);
    }
    
    private static bool CanShow(InstanceItemViewModel instance) => instance.CanShow;
    
    [RelayCommand(CanExecute = nameof(CanShow))]
    private void Show(InstanceItemViewModel instance)
    {
        if (!CanShow(instance))
            return;
        
        _instanceService.Show(instance.Id);
    }
    
    private bool CanLaunchSelected => SelectedInstances.Items.Any(CanLaunch);
    
    [RelayCommand(CanExecute = nameof(CanLaunchSelected))]
    private async Task LaunchSelected()
    {
        var launchTasks = SelectedInstances.Items
            .Where(CanLaunch)
            .Select(i => _instanceService.LaunchAsync(i.Id));
        await Task.WhenAll(launchTasks);
    }
    
    private bool CanStopSelected => SelectedInstances.Items.Any(CanStop);

    [RelayCommand(CanExecute = nameof(CanStopSelected))]
    private async Task StopSelected()
    {
        var stopTasks = SelectedInstances.Items
            .Where(CanStop)
            .Select(i => _instanceService.StopAsync(i.Id));
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
