using System.Threading.Tasks;
using AvaloniaApplication1.Secret.Requests;

namespace AvaloniaApplication1.Secret;

public interface ISecretMigrationCoordinator
{
    Task MigrateAsync(EncryptionRequest request);
}