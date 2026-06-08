using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public class RegionsPageDesignViewModel : RegionsPageViewModel
{
    public RegionsPageDesignViewModel() : base(DesignServices.RegionService)
    {
        this.EditRegionCommand.Execute(Regions[2]);
    }
    

}
