using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using RegionsPageViewModel = AvaloniaApplication1.ViewModels.RegionsPageViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public sealed class RegionsPageDesignViewModel : RegionsPageViewModel
{
    public RegionsPageDesignViewModel() : base(DesignServices.RegionService)
    {
        OnEnter();
        EditCommand.Execute(Cards[2]);
    }
}
