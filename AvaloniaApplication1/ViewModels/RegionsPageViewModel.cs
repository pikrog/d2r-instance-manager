using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels.Dialog;
using AvaloniaApplication1.ViewModels.Region;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class RegionsPageViewModel : ViewModelBase, IDialogParticipant
{
    private readonly RegionService _regionService;

    public ObservableCollection<RegionCardViewModel> Regions { get; } = [];

    public bool IsNotEditing => EditedCard is null;
    
    [NotifyCanExecuteChangedFor(nameof(NewCommand), nameof(EditCommand), nameof(DeleteCommand))]
    [ObservableProperty]
    public partial RegionCardViewModel? EditedCard { get; set; }

    public RegionsPageViewModel(RegionService regionService)
    {
        _regionService = regionService;
        Refresh();
    }
    
    public void Refresh()
    {
        // todo: rely on service event instead of polling
        
        var regions = _regionService.GetAllSnapshots().ToList(); // todo: Get Snapshot Projection for UI? RegionTableRow
        
        var i = 0;
        for (; i < Math.Min(regions.Count, Regions.Count); i++)
        {
            Regions[i].Id = regions[i].Id;
            Regions[i].Name = regions[i].Name;
            Regions[i].Address = regions[i].Address;
        }
        
        for(; i < regions.Count; i++)
            Regions.Add(new RegionCardViewModel(regions[i].Id, regions[i].Name, regions[i].Address));
        
        for (; i < Regions.Count; i++)
            Regions.RemoveAt(i);
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void New()
    {
        var region = new RegionCardViewModel();
        Regions.Add(region);
        Edit(region);
    }

    [RelayCommand]
    private async Task Save(EditRegionFormViewModel form)
    {
        if (!form.Validate())
            return;
        
        var draft = new RegionDraft(form.Id, form.Name, form.Address);
        await _regionService.SaveAsync(draft);
        
        EndEdit();
        Refresh(); // [optional] todo: OnRegionsChanged from Service with event type Added/Updated
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void Edit(RegionCardViewModel region)
    {
        EndEdit();
        EditedCard = region;
        EditedCard.OpenForm();
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private async Task Delete(RegionCardViewModel region)
    {
        EndEdit();
        await _regionService.RemoveAsync(region.Id!.Value);
        Refresh(); // [optional] todo: OnRegionsChanged from Service with event type Removed
    }

    [RelayCommand]
    private void EndEdit()
    {
        EditedCard?.CloseForm();
        if (EditedCard is not null && EditedCard.Id is null)
            Regions.Remove(EditedCard);
        EditedCard = null;
    }
}
