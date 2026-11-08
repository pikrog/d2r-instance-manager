using System.Collections.ObjectModel;
using D2RInstanceManager.Toast;
using D2RInstanceManager.Toast.Descriptors;

namespace D2RInstanceManager.Design.Services;

public class FakeToastHost : ToastHost
{
    public FakeToastHost()
    {
        Add(new ToastNotification(new GameExecutableFound(@"D:\Program Files (x86)\Diablo II Resurrected\D2R.exe")));
        Add(new ToastNotification(GameExecutableNotFound.Instance));
    }
}