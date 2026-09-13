using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Navigation;

namespace AvaloniaApplication1.Log;

public class LogsPageViewModel : PageViewModel
{
    private readonly ILogStoreReader _store;
    
    private IDisposable? _subscriptionHandle;

    private readonly ObservableCollection<LogEntry> _entries = [];

    public ReadOnlyObservableCollection<LogEntry> Entries { get; }
    
    public LogsPageViewModel(ILogStoreReader store)
    {
        _store = store;
        
        Entries = new ReadOnlyObservableCollection<LogEntry>(_entries);
    }


    private void AddEntry(LogEntry entry)
    {
        _entries.Add(entry);
    }
    
    private void OnNextEntry(LogEntry entry)
    {
        Dispatcher.UIThread.Post(() => AddEntry(entry));
    }
    
    public override Task OnEnterAsync()
    {
        var (snapshot, handle) = _store.Subscribe(OnNextEntry);
        
        _subscriptionHandle = handle;
        
        _entries.Clear();
        foreach (var entry in snapshot)
            AddEntry(entry);
        
        return Task.CompletedTask;
    }

    public override Task OnLeaveAsync()
    {
        _subscriptionHandle?.Dispose();
        _subscriptionHandle = null;
        
        return Task.CompletedTask;
    }
}