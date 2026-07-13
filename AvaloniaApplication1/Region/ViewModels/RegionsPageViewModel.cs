using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Card;
using AvaloniaApplication1.Card.ViewModels;
using AvaloniaApplication1.Dialog;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Overlay.Dialog.DiscardChanges;
using AvaloniaApplication1.Page;
using AvaloniaApplication1.Region.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Region.ViewModels;

public partial class RegionsPageViewModel : PageViewModel, IDialogParticipant
{
    private readonly RegionService _regionService;
    
    private readonly OverlayService _overlayService;

    private readonly ObservableCollection<RegionCardViewModel> _regions = [];

    private readonly CardCollection<RegionCardViewModel> _cards;
    
    public ReadOnlyObservableCollection<CardViewModel> Cards => _cards.Cards;

    public bool IsNotEditing => EditedCard is null;
    
    [NotifyCanExecuteChangedFor(nameof(NewCommand), nameof(EditCommand), nameof(DeleteCommand))]
    [ObservableProperty]
    public partial RegionCardViewModel? EditedCard { get; set; }
    
    public RegionsPageViewModel(RegionService regionService, OverlayService overlayService)
    {
        _regionService = regionService;
        _overlayService = overlayService;

        _cards = new CardCollection<RegionCardViewModel>(_regions);
    }
    
    public void Refresh()
    {
        var regions = _regionService.GetSummaries()
            .Select(summary => new RegionCardViewModel(summary.Id, summary.Name, summary.Address, summary.InstanceCount))
            .ToList();
        
        var i = 0;
        for (; i < Math.Min(regions.Count, _regions.Count); i++)
        {
            var savedCard = _regions[i];
            savedCard.Id = regions[i].Id;
            savedCard.Name = regions[i].Name;
            savedCard.Address = regions[i].Address;
            savedCard.InstanceCount = regions[i].InstanceCount;
        }

        for(; i < regions.Count; i++)
            _regions.Add(regions[i]);

        for (; i < _regions.Count; )
            _regions.RemoveAt(i);
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private void New()
    {
        var region = new RegionCardViewModel();
        _regions.Add(region);
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
        EditedCard = region;
        EditedCard.OpenForm();
    }

    [RelayCommand(CanExecute = nameof(IsNotEditing))]
    private async Task Delete(RegionCardViewModel region)
    {
        var confirmationViewModel = new DeleteRegionDialogViewModel(region.Name, region.InstanceCount);
        var result = await _overlayService.ShowAsync(confirmationViewModel);
        if (!result)
            return;
        await _regionService.RemoveAsync(region.Id!.Value);
        Refresh(); // [optional] todo: OnRegionsChanged from Service with event type Removed
    }

    [RelayCommand]
    private void EndEdit()
    {
        EditedCard?.CloseForm();
        if (EditedCard is not null && EditedCard.Id is null)
            _regions.Remove(EditedCard);
        EditedCard = null;
    }

    public override Task OnEnterAsync()
    {
        Refresh();
        return Task.CompletedTask;
    }
    
    public override async Task<bool> CanLeaveAsync()
    {
        if (EditedCard is null)
            return true;

        var confirmationViewModel = new DiscardChangesDialogViewModel();
        return await _overlayService.ShowAsync(confirmationViewModel);
    }

    public override Task OnLeaveAsync()
    {
        EndEdit();
        return Task.CompletedTask;
    }
}
