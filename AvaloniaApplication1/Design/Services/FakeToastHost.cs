using System.Collections.ObjectModel;
using AvaloniaApplication1.Toast;
using AvaloniaApplication1.Toast.Descriptors;

namespace AvaloniaApplication1.Design.Services;

public class FakeToastHost : ToastHost
{
    public FakeToastHost()
    {
        Add(new ToastNotification(new GameExecutableFound(@"D:\Program Files (x86)\Diablo II Resurrected\D2R.exe")));
        Add(new ToastNotification(GameExecutableNotFound.Instance));
    }
}