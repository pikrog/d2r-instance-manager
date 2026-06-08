using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;

namespace AvaloniaApplication1.ViewModels;

public partial class RegionsPageViewModel : ViewModelBase, IDialogParticipant
{
    private readonly RegionService _regionService;

    public ObservableCollection<RegionCardViewModel> Regions { get; } = [];

    public bool IsNotEditing => EditedCard is null;
    
    [NotifyCanExecuteChangedFor(nameof(NewRegionCommand), nameof(EditRegionCommand), nameof(DeleteRegionCommand))]
    [ObservableProperty]
    public partial RegionCardViewModel? EditedCard { get; set; }

    public RegionsPageViewModel(RegionService regionService)
    {
        _regionService = regionService;
        Populate();
    }
    
    public void Populate()
    {
        Regions.Clear();

        var regions = _regionService.GetAllSnapshots().Select(r => new RegionCardViewModel(r.Id, r.Name, r.Address));
        Regions.AddRange(regions);
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void NewRegion()
    {
        var region = new RegionCardViewModel();
        Regions.Add(region);
        EditRegion(region);
    }

    [RelayCommand]
    private async Task SaveRegion(EditRegionFormViewModel form)
    {
        if (!form.Validate())
            return;
        
        var draft = new RegionDraft(form.Id, form.Name, form.Address);
        await _regionService.Save(draft);
        
        EndEdit();
        Populate(); // [optional] todo: OnRegionsChanged from Service with event type Added/Updated
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void EditRegion(RegionCardViewModel region)
    {
        EndEdit();
        EditedCard = region;
        EditedCard.OpenForm();
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private async Task DeleteRegion(RegionCardViewModel region)
    {
        EndEdit();
        await _regionService.Remove(region.Id!.Value);
        Populate(); // [optional] todo: OnRegionsChanged from Service with event type Removed
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
