using AvaloniaApplication1.Toast.Descriptors;

namespace AvaloniaApplication1.Toast;

public class ToastService(IToastController controller)
{
    public void Show(ToastDescriptor descriptor)
    {
        var toast = new ToastNotification(descriptor);
        controller.Add(toast);
    }
}