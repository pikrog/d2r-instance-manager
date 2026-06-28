using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using RegionsPageViewModel = AvaloniaApplication1.ViewModels.RegionsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class RegionsPageDesignViewModel : RegionsPageViewModel
{
    public RegionsPageDesignViewModel() : base(DesignServices.RegionService)
    {
        this.EditCommand.Execute(Regions[2]);
    }
    

}
