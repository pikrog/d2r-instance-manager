using System.Linq;
using D2RInstanceManager.Config;
using D2RInstanceManager.Secret;

namespace D2RInstanceManager.Account;

public static class AccountSecretMigrationService
{
    public static void Migrate(ConfigContext context, ISecretTransformer transformer)
    {
        var migratedAccounts = context.GetAllAccounts()
            .Select(
                a => a with
                {
                    Password = transformer.Transform(a.Password)
                })
            .ToList();
            
        foreach (var account in migratedAccounts)
            context.UpdateAccount(account);
    }
}