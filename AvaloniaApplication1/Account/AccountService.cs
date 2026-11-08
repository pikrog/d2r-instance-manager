using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Config;

namespace D2RInstanceManager.Account;

public class AccountService(ConfigService configService, AccountSnapshotTransformer transformer)
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
        var runtimeSnapshot = new AccountRuntimeSnapshot(id, displayName, draft.Username, draft.Password);
        
        var configSnapshot = transformer.ToConfigSnapshot(runtimeSnapshot);
        
        if (draft.Id is null)
            await AddAsync(configSnapshot);
        else
            await UpdateAsync(configSnapshot);
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
            .Select(transformer.ToRuntimeSnapshot)
            .Select(a => 
                new AccountSummary(a.Id,
                    a.DisplayName,
                    a.Username, 
                    a.Password,
                    instanceCounts.GetValueOrDefault(a.Id, 0))
            )
            .ToList();
    }

    public bool Exists(Guid id) => configService.Config.AccountExists(id);
}