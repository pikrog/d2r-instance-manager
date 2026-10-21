using System;
using System.Collections.ObjectModel;

namespace AvaloniaApplication1.Toast;

public interface IToastHost
{
    ReadOnlyObservableCollection<ToastNotification> Toasts { get; }
    IDisposable SuspendToasts();
}