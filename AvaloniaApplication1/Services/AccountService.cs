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
    private async Task AddAsync(AccountSnapshot snapshot) => 
        await configService.ChangeAsync(context => context.AddAccount(snapshot));

    private async Task UpdateAsync(AccountSnapshot snapshot) => 
        await configService.ChangeAsync(context => context.UpdateAccount(snapshot));

    public async Task SaveAsync(AccountDraft draft)
    {
        // Validate(draft);

        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new AccountSnapshot(id, draft.Username, draft.Password);
        
        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }

    public async Task RemoveAsync(Guid id) => await configService.ChangeAsync(context => context.RemoveAccount(id));

    public AccountSnapshot GetSnapshot(Guid id) => configService.Config.GetAccount(id);
    
    public IReadOnlyList<AccountSnapshot> GetAllSnapshots() => configService.Config.GetAllAccounts();
    
    public IReadOnlyList<AccountOption> GetOptions() => 
        GetAllSnapshots().Select(a => new AccountOption(a.Id, a.Username)).ToList();

    public bool Exists(Guid id) => configService.Config.AccountExists(id);
}