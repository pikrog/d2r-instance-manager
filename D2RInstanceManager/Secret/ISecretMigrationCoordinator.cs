using System.Threading.Tasks;
using D2RInstanceManager.Secret.Requests;

namespace D2RInstanceManager.Secret;

public interface ISecretMigrationCoordinator
{
    Task MigrateAsync(EncryptionRequest request);
}