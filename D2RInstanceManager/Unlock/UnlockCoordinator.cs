using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using D2RInstanceManager.Secret;

namespace D2RInstanceManager.Unlock;

public class UnlockCoordinator(
    IUnlockService service,
    UnlockWindowViewModel viewModel,
    INonKeyBasedSecretProtectionVerifier verifier)
{
    public async Task<bool> UnlockAsync(Window owner)
    {
        if (!service.RequiresUnlock)
        {
            return verifier.VerifyNonKeyBased() 
                ? true 
                : throw new InvalidOperationException("Failed to verify non-key-based secret protection");
        }

        var window = new UnlockWindowView
        {
            DataContext = viewModel,
        };

        var dialogTask = Dispatcher.UIThread.InvokeAsync(() => window.ShowDialog(owner));
        var resultTask = viewModel.IsUnlocked;

        await Task.WhenAny(dialogTask, resultTask);
        
        window.Close();
        
        await dialogTask;

        return await resultTask;
    }
}