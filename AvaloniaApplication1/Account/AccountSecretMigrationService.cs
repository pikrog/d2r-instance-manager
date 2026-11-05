using System.Linq;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Secret;

namespace AvaloniaApplication1.Account;

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