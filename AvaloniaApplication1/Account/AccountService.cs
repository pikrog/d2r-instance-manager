using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Account;

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
        var displayName = string.IsNullOrWhiteSpace(draft.DisplayName) ? null : draft.DisplayName;
        var snapshot = new AccountSnapshot(id, displayName, draft.Username, draft.Password);
        
        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }

    public async Task RemoveAsync(Guid id) => await configService.ChangeAsync(context => context.RemoveAccount(id));

    public AccountSnapshot GetSnapshot(Guid id) => configService.Config.GetAccount(id);
    
    public IReadOnlyList<AccountSnapshot> GetAllSnapshots() => configService.Config.GetAllAccounts();
    
    public IReadOnlyList<AccountOption> GetOptions() => 
        GetAllSnapshots().Select(a => new AccountOption(a.Id, a.DisplayName, a.Username)).ToList();

    public IReadOnlyList<AccountSummary> GetSummaries()
    {
        var instanceCounts = configService.Config.GetAllInstances()
            .Where(i => i.AccountId.HasValue)
            .GroupBy(i => i.AccountId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
        return GetAllSnapshots()
            .Select(a => 
                new AccountSummary(
                    a.Id,
                    a.DisplayName,
                    a.Username, 
                    a.Password,
                    instanceCounts.GetValueOrDefault(a.Id, 0)
                    )
            )
            .ToList();
    }

    public bool Exists(Guid id) => configService.Config.AccountExists(id);
}