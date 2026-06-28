using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels.Dialog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class RegionsPageViewModel(RegionService regionService) : PageViewModel, IDialogParticipant
{
    public ObservableCollection<CardViewModel> Cards { get; } = [];//[new AddCardViewModel()];
    
    private int EntityCardsCount => Cards.Count;

    public bool IsNotEditing => EditedCard is null;
    
    [NotifyCanExecuteChangedFor(nameof(NewCommand), nameof(EditCommand), nameof(DeleteCommand))]
    [ObservableProperty]
    public partial RegionCardViewModel? EditedCard { get; set; }

    private void AddEntityCard(RegionCardViewModel card)
    {
        Cards.Insert(EntityCardsCount, card);
    }
    
    public void Refresh()
    {
        var regions = regionService.GetAllSnapshots()
            .ToList(); // todo: Get Snapshot Projection for UI? RegionTableRow
        
        var i = 0;
        for (; i < Math.Min(regions.Count, EntityCardsCount); i++)
        {
            var card = (Cards[i] as RegionCardViewModel)!;
            card.Id = regions[i].Id;
            card.Name = regions[i].Name;
            card.Address = regions[i].Address;
        }

        for(; i < regions.Count; i++)
            AddEntityCard(new RegionCardViewModel(regions[i].Id, regions[i].Name, regions[i].Address));

        for (; i < EntityCardsCount; )
            Cards.RemoveAt(i);
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void New()
    {
        var region = new RegionCardViewModel();
        AddEntityCard(region);
        Edit(region);
    }

    [RelayCommand]
    private async Task Save(EditRegionFormViewModel form)
    {
        if (!form.Validate())
            return;
        
        var draft = new RegionDraft(form.Id, form.Name, form.Address);
        await regionService.SaveAsync(draft);
        
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
        await regionService.RemoveAsync(region.Id!.Value);
        Refresh(); // [optional] todo: OnRegionsChanged from Service with event type Removed
    }

    [RelayCommand]
    private void EndEdit()
    {
        EditedCard?.CloseForm();
        if (EditedCard is not null && EditedCard.Id is null)
            Cards.Remove(EditedCard);
        EditedCard = null;
    }

    public override void OnEnter()
    {
        Refresh();
    }

    public override bool OnLeave()
    {
        // todo: save changes?
        return true;
    }
}
