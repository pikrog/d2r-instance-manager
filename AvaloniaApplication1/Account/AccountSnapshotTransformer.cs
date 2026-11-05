using System.Text;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Secret;

namespace AvaloniaApplication1.Account;

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