using System.Linq;
using System.Threading.Tasks;
using D2RInstanceManager.Account;
using D2RInstanceManager.Display;
using D2RInstanceManager.HotKey.Config;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Region;

namespace D2RInstanceManager.Instance.ViewModels;

public class EditInstanceFormViewModelFactory(
    IHotKeyConfigValidator hotKeyConfigValidator, 
    AccountService accountService,
    RegionService regionService, 
    DisplayService displayService
    )
{
    public EditInstanceFormViewModel Create(InstanceSnapshot? snapshot) => 
        snapshot is null ? CreateNew() : CreateEdit(snapshot);

    private EditInstanceFormViewModel CreateNew()
    {
        var accountOptions = accountService.GetOptions();
        var regionOptions = regionService.GetOptions();
        var displayOptions = displayService.GetOptions();
        return new EditInstanceFormViewModel(hotKeyConfigValidator, accountOptions, regionOptions, displayOptions)
        {
            SelectedDisplay = displayOptions[0]
        };
    }

    private EditInstanceFormViewModel CreateEdit(InstanceSnapshot snapshot)
    {
        var accountOptions = accountService.GetOptions();
        var regionOptions = regionService.GetOptions();
        var displayOptions = displayService.GetOptions(snapshot.Display as DisplaySelection.Specific);
        
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