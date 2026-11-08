using D2RInstanceManager.Design.Services;
using D2RInstanceManager.Region.ViewModels;

namespace D2RInstanceManager.Design.ViewModels;

public sealed class RegionsPageDesignViewModel : RegionsPageViewModel
{
    public RegionsPageDesignViewModel() : base(DesignServices.RegionService, DesignServices.OverlayService)
    {
        Refresh();
        EditCommand.Execute(Cards[2]);
    }
}
