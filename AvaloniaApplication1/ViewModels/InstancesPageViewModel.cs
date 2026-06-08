using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.Snapshots;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;

namespace AvaloniaApplication1.ViewModels;

public partial class InstancesPageViewModel : ViewModelBase, IDialogParticipant
{
    private readonly GameInstanceService _gameInstanceService;

    private readonly AccountService _accountService;
    
    private readonly RegionService _regionService;

    public ObservableCollection<GameInstanceTableRow> Instances { get; } = [];
    
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(IsInstanceSelected))]
    [NotifyCanExecuteChangedFor(nameof(EditInstanceCommand), nameof(RemoveInstanceCommand))]
    public partial GameInstanceTableRow? SelectedInstance { get; set; }
    
    public bool IsInstanceSelected => SelectedInstance is not null;
    
    public InstancesPageViewModel(GameInstanceService gameInstanceService,
        AccountService accountService,
        RegionService regionService)
    {
        _gameInstanceService = gameInstanceService;
        _accountService = accountService;
        _regionService = regionService;
        
        Populate();
    }
    
    private void Populate()
    {
        Instances.Clear();
        var table = _gameInstanceService.GetTable();
        Instances.AddRange(table);
    }

    private EditInstanceFormViewModel CreateForm(GameInstanceSnapshot? snapshot = null)
    {
        var accountOptions = _accountService.GetOptions();
        var regionOptions = _regionService.GetOptions();
        var displayOptions = new List<int> { 1, 2 }; // todo: display service
        var form =  new EditInstanceFormViewModel(accountOptions, regionOptions, displayOptions);

        if (snapshot is null)
            return form;
        
        var selectedAccount = accountOptions.SingleOrDefault(x => x.Id == snapshot.AccountId);
        var selectedRegion = regionOptions.SingleOrDefault(x => x.Id == snapshot.RegionId);
        var selectedCredentialsVector = EditInstanceFormViewModel.CredentialsVectorOptions.SingleOrDefault(o => o.CredentialsVector == snapshot.CredentialsVector);
        form.Id = snapshot.Id;
        form.Name = snapshot.Name;
        form.IsOnlineMode = snapshot.IsOnlineMode;
        form.SelectedAccount = selectedAccount;
        form.SelectedCredentialsVector = selectedCredentialsVector;
        form.SelectedRegion = selectedRegion;
        form.Display = snapshot.DisplayId;
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
            form.Display,
            form.IsNoSound,
            form.IsWindowedMode, 
            form.RecallHotKey
        );
    }
    
    private async Task CoreEditInstance(GameInstanceSnapshot? snapshot = null)
    {
        var form = CreateForm(snapshot);
        var okPressed = await this.OpenForm(form);
        if (!okPressed)
            return;
        var draft = CreateDraft(form);
        await _gameInstanceService.Save(draft);
        Populate();
    }
    
    [RelayCommand]
    private async Task NewInstance()
    {
        await EditInstance();
    }
    
    [RelayCommand(CanExecute = nameof(IsInstanceSelected))]
    private async Task EditInstance() // todo: pass instance or no args and just get selected item?
    {
        if (SelectedInstance is null)
            return;
        var snapshot = _gameInstanceService.GetInstanceSnapshot(SelectedInstance.Id);
        await CoreEditInstance(snapshot);
    }
    
    [RelayCommand(CanExecute = nameof(IsInstanceSelected))]
    private async Task RemoveInstance()
    {
        if (SelectedInstance is null)
            return;
        await _gameInstanceService.Remove(SelectedInstance.Id);
        Populate();
    }


}
