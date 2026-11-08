using System.Text;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Secret;

namespace D2RInstanceManager.Account;

public class AccountSnapshotTransformer(ISecretProtectorFactory secretProtectorFactory)
{
    public AccountRuntimeSnapshot ToRuntimeSnapshot(AccountSnapshot snapshot)
    {
        var protector = secretProtectorFactory.CreateCurrent();
        var passwordBytes = protector.Unprotect(snapshot.Password);
        var password = Encoding.UTF8.GetString(passwordBytes);
        return new AccountRuntimeSnapshot(snapshot.Id, snapshot.DisplayName, snapshot.Username, password);
    }
    
    public AccountSnapshot ToConfigSnapshot(AccountRuntimeSnapshot snapshot)
    {
        var protector = secretProtectorFactory.CreateCurrent();
        var passwordBytes = Encoding.UTF8.GetBytes(snapshot.Password);
        var secretData = protector.Protect(passwordBytes);
        return new AccountSnapshot(snapshot.Id, snapshot.DisplayName, snapshot.Username, secretData);
    }
}