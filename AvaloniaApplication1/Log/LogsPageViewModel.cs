using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;

namespace AvaloniaApplication1.Log;

public partial class LogsPageViewModel : PageViewModel
{
    private readonly ILogStoreReader _store;

    private readonly IInstanceConfigProvider _instanceConfigProvider;
    
    private readonly ILogService _logService;
    
    private IDisposable? _subscriptionHandle;

    private readonly ObservableCollection<LogEntry> _entries = [];
    
    private readonly ObservableCollection<LogEntry> _filteredEntries = [];
    
    public ReadOnlyObservableCollection<LogEntry> FilteredEntries { get; }
    
    public LogFilter Filter { get; private set; }

    [ObservableProperty] 
    public partial string? SearchText { get; set; }

    [ObservableProperty] 
    public partial LogLevel Level { get; set; } = LogLevel.Information;
    
    public IReadOnlyList<LogLevel> AvailableLogLevelOptions { get; }

    public NameFilter InstanceFilter { get; } = new();
    
    public NameFilter SourceFilter { get; } = new();
    
    public LogsPageViewModel(ILogStoreReader store, LogSettings logSettings, ILogService logService, IInstanceConfigProvider instanceConfigProvider)
    {
        _store = store;
        _instanceConfigProvider = instanceConfigProvider;
        _logService = logService;
        
        FilteredEntries = new ReadOnlyObservableCollection<LogEntry>(_filteredEntries);

        AvailableLogLevelOptions = Enum.GetValues<LogLevel>()
            .Where(l => LogLevels.IsEnabled(l, logSettings.MinimumLevel))
            .ToArray();

        InstanceFilter.ActiveSelections.CollectionChanged += OnNameFilterActiveSelectionsChanged;
        SourceFilter.ActiveSelections.CollectionChanged += OnNameFilterActiveSelectionsChanged;

        Filter = CreateFilter();
    }

    private void OnNameFilterActiveSelectionsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        UpdateFilter();

    private LogFilter CreateFilter() =>
        new(Level,
            InstanceFilter.ActiveNames,
            InstanceFilter.IsNoNameActive,
            SourceFilter.ActiveNames,
            SourceFilter.IsNoNameActive,
            SearchText);

    private void UpdateFilter()
    {
        Filter = CreateFilter();
        
        RefreshEntries();
    }

    private void AddEntry(LogEntry entry)
    {
        _entries.Add(entry);
        
        if (_logService.IsEntryVisible(entry, Filter))
            _filteredEntries.Add(entry);
        
        if (!string.IsNullOrEmpty(entry.SourceModule))
            SourceFilter.AddName(entry.SourceModule);
    }
    
    private void OnNextEntry(LogEntry entry)
    {
        Dispatcher.UIThread.Post(() => AddEntry(entry));
    }

    private void RefreshEntries()
    {
        var entries = _logService.Filter(_entries, Filter);
        
        _filteredEntries.Clear();
        _filteredEntries.AddRange(entries);
    }

    partial void OnLevelChanged(LogLevel value) => UpdateFilter();

    partial void OnSearchTextChanged(string? value) => UpdateFilter();

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
        var instances = _instanceConfigProvider.GetConfigSnapshots()
            .Select(i => i.Name)
            .ToList();
        
        InstanceFilter.ClearNames();
        foreach (var instance in instances)
            InstanceFilter.AddName(instance);
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