using D2RInstanceManager.Toast.Descriptors;

namespace D2RInstanceManager.Toast;

public class ToastService(IToastController controller)
{
    public void Show(ToastDescriptor descriptor)
    {
        var toast = new ToastNotification(descriptor);
        controller.Add(toast);
    }
}