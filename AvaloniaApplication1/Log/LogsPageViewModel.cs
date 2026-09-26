using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Log.Instance;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Selectable;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Log;

public partial class LogsPageViewModel : PageViewModel
{
    private readonly ILogStoreReader _store;

    private readonly InstanceService _instanceService;
    
    private IDisposable? _subscriptionHandle;

    private readonly ObservableCollection<LogEntry> _entries = [];

    public ReadOnlyObservableCollection<LogEntry> Entries { get; }
    
    private readonly ObservableCollection<LogEntry> _filteredEntries = [];
    
    public ReadOnlyObservableCollection<LogEntry> FilteredEntries { get; }

    [ObservableProperty] 
    public partial LogLevel Level { get; set; } = LogLevel.Information;
    
    public IReadOnlyList<LogLevel> AvailableLogLevels { get; }
    
    private readonly NoInstanceFilterSelection _noInstanceFilterSelection = new();

    private readonly ObservableCollection<InstanceFilterSelection> _instanceFilterSelections = [];

    public ReadOnlyObservableCollection<InstanceFilterSelection> InstanceFilterSelections { get; }

    public SelectedItemsCollection<InstanceFilterSelection> InstanceFilterActiveSelections { get; }
    
    public bool IsOnlyNoInstanceFilterActive => 
        InstanceFilterActiveSelections.Count == 1 && _noInstanceFilterSelection.IsSelected;

    public bool IsOnlyNamedInstanceFilterActive =>
        InstanceFilterActiveSelections.Any && !_noInstanceFilterSelection.IsSelected;
    
    public bool IsMixedFilteringActive =>
        InstanceFilterActiveSelections.Count > 1 && _noInstanceFilterSelection.IsSelected;
    
    public LogsPageViewModel(ILogStoreReader store, LogSettings logSettings, InstanceService instanceService)
    {
        _store = store;
        _instanceService = instanceService; // todo: replace with LogService
        
        Entries = new ReadOnlyObservableCollection<LogEntry>(_entries);
        FilteredEntries = new ReadOnlyObservableCollection<LogEntry>(_filteredEntries);

        AvailableLogLevels = Enum.GetValues<LogLevel>()
            .Where(l => LogLevels.IsEnabled(l, logSettings.MinimumLevel))
            .ToArray();
        
        InstanceFilterSelections = new ReadOnlyObservableCollection<InstanceFilterSelection>(_instanceFilterSelections);
        InstanceFilterActiveSelections = new SelectedItemsCollection<InstanceFilterSelection>(InstanceFilterSelections);
        InstanceFilterActiveSelections.CollectionChanged += OnInstanceFilterActiveSelectionsChanged; 
    }

    private void OnInstanceFilterActiveSelectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshEntries();
        OnPropertyChanged(nameof(IsOnlyNoInstanceFilterActive));
        OnPropertyChanged(nameof(IsOnlyNamedInstanceFilterActive));
        OnPropertyChanged(nameof(IsMixedFilteringActive));
    }

    private void AddEntry(LogEntry entry)
    {
        _entries.Add(entry);
        
        if (LogLevels.IsVisible(entry.Level, Level))
            _filteredEntries.Add(entry);
    }
    
    private void OnNextEntry(LogEntry entry)
    {
        Dispatcher.UIThread.Post(() => AddEntry(entry));
    }

    private bool IsEntryVisibleByInstance(string? instanceName) =>
        !InstanceFilterActiveSelections.Any
        || (string.IsNullOrEmpty(instanceName) 
            ? _noInstanceFilterSelection.IsSelected
            : InstanceFilterActiveSelections.Items
                .Any(i => (i as NamedInstanceFilterSelection)?.Name == instanceName));

    private bool IsEntryVisible(LogEntry entry) => 
        LogLevels.IsVisible(entry.Level, Level) && IsEntryVisibleByInstance(entry.InstanceName);

    private void RefreshEntries()
    {
        _filteredEntries.Clear();
        foreach (var entry in _entries)
        {
            if (IsEntryVisible(entry))
                _filteredEntries.Add(entry);
        }
    }
    
    partial void OnLevelChanged(LogLevel value)
    {
        RefreshEntries();
    }

    private void SubscribeAndFetchLogs()
    {
        _subscriptionHandle?.Dispose();
        
        var (snapshot, handle) = _store.Subscribe(OnNextEntry);
        
        _subscriptionHandle = handle;
        
        _entries.Clear();
        _filteredEntries.Clear();
        foreach (var entry in snapshot)
            AddEntry(entry);
    }

    private void FillInstancesFilter()
    {
        var instances = _instanceService.GetConfigSnapshots()
            .Select(i => new NamedInstanceFilterSelection(i.Name))
            .ToList();
        
        _instanceFilterSelections.Clear();
        foreach (var instance in instances)
            _instanceFilterSelections.Add(instance);

        _instanceFilterSelections.Add(_noInstanceFilterSelection);
    }
    
    public override Task OnEnterAsync()
    {
        SubscribeAndFetchLogs();
        
        FillInstancesFilter();
        
        return Task.CompletedTask;
    }

    public override Task OnLeaveAsync()
    {
        _subscriptionHandle?.Dispose();
        _subscriptionHandle = null;
        
        return Task.CompletedTask;
    }
}