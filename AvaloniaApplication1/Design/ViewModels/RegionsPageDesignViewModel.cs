using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Region.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public sealed class RegionsPageDesignViewModel : RegionsPageViewModel
{
    public RegionsPageDesignViewModel() : base(DesignServices.RegionService, DesignServices.OverlayService)
    {
        Refresh();
        EditCommand.Execute(Cards[2]);
    }
}
