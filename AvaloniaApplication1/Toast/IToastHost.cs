using System;
using System.Collections.ObjectModel;

namespace D2RInstanceManager.Toast;

public interface IToastHost
{
    ReadOnlyObservableCollection<ToastNotification> Toasts { get; }
    IDisposable SuspendToasts();
}