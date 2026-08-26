using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Account;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.HotKey.Config;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region;

namespace AvaloniaApplication1.Instance.ViewModels;

public class EditInstanceFormViewModelFactory(
    IHotKeyConfigValidator hotKeyConfigValidator, 
    AccountService accountService,
    RegionService regionService, 
    DisplayService displayService
    )
{
    public Task<EditInstanceFormViewModel> CreateAsync(InstanceSnapshot? snapshot) => 
        snapshot is null ? CreateNew() : CreateEdit(snapshot);

    private async Task<EditInstanceFormViewModel> CreateNew()
    {
        var accountOptions = accountService.GetOptions();
        var regionOptions = regionService.GetOptions();
        var displayOptions = await displayService.GetOptionsAsync();
        return new EditInstanceFormViewModel(hotKeyConfigValidator, accountOptions, regionOptions, displayOptions)
        {
            SelectedDisplay = displayOptions[0]
        };
    }

    private async Task<EditInstanceFormViewModel> CreateEdit(InstanceSnapshot snapshot)
    {
        var accountOptions = accountService.GetOptions();
        var regionOptions = regionService.GetOptions();
        var displayOptions = await displayService.GetOptionsAsync(snapshot.Display as DisplaySelection.Specific);
        
        var selectedAccount = accountOptions.SingleOrDefault(x => x.Id == snapshot.AccountId);
        var selectedRegion = regionOptions.SingleOrDefault(x => x.Id == snapshot.RegionId);
        var selectedAuthenticationMethod = EditInstanceFormViewModel.AuthenticationMethodOptions
            .SingleOrDefault(o => o.AuthenticationMethod == snapshot.AuthenticationMethod);
        var selectedDisplay = displayOptions.SingleOrDefault(o => DisplayMatcher.IsMatch(snapshot.Display, o)) 
                              ?? DisplayOption.Default;
        
        return new EditInstanceFormViewModel(hotKeyConfigValidator, accountOptions, regionOptions, displayOptions)
        {
            Id = snapshot.Id,
            Name = snapshot.Name,
            IsOnlineMode = snapshot.IsOnlineMode,
            SelectedAccount = selectedAccount,
            SelectedRegion = selectedRegion,
            SelectedAuthenticationMethod = selectedAuthenticationMethod,
            SelectedDisplay = selectedDisplay,
            ShowCommandHotKey = snapshot.ShowCommandHotKey,
            IsNoSound = snapshot.IsNoSound,
            IsWindowedMode = snapshot.IsWindowedMode
        };
    }
            
}