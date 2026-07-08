using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using RegionsPageViewModel = AvaloniaApplication1.ViewModels.Region.RegionsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public sealed class RegionsPageDesignViewModel : AvaloniaApplication1.ViewModels.Region.RegionsPageViewModel
{
    public RegionsPageDesignViewModel() : base(DesignServices.RegionService, DesignServices.OverlayService)
    {
        OnEnter();
        EditCommand.Execute(Cards[2]);
    }
}
