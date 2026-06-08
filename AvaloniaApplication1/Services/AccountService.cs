using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Services;

public class AccountService(ConfigService configService)
{
    private async Task Add(AccountSnapshot snapshot) => 
        await configService.ChangeAsync(context => context.AddAccount(snapshot));

    private async Task Update(AccountSnapshot snapshot) => 
        await configService.ChangeAsync(context => context.UpdateAccount(snapshot));

    public async Task Save(AccountDraft draft)
    {
        // Validate(draft);

        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new AccountSnapshot(id, draft.Username, draft.Password);
        
        if (draft.Id is null)
            await Add(snapshot);
        else
            await Update(snapshot);
    }

    public async Task Remove(Guid id) => await configService.ChangeAsync(context => context.RemoveAccount(id));

    public AccountSnapshot GetSnapshot(Guid id) => configService.Config.GetAccount(id);
    
    public IReadOnlyList<AccountSnapshot> GetAllSnapshots() => configService.Config.GetAllAccounts();
    
    public List<AccountOption> GetOptions() => 
        GetAllSnapshots().Select(a => new AccountOption(a.Id, a.Username)).ToList();

    public bool Exists(Guid id) => configService.Config.AccountExists(id);
}