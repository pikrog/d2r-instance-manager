using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Notification;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Log;

public partial class LogsPageViewModel : PageViewModel
{
    private readonly ILogStoreReader _store;

    private readonly IInstanceConfigProvider _instanceConfigProvider;
    
    private readonly ILogService _logService;
    
    private readonly INotificationAcknowledger _notificationAcknowledger;
    
    private readonly ILogger<LogsPageViewModel> _logger;
    
    private IDisposable? _subscriptionHandle;

    private int _subscriptionGeneration;

    private readonly List<LogEntry> _entries = [];
    
    private readonly ObservableCollectionExtended<LogEntry> _filteredEntries = [];
    
    public ReadOnlyObservableCollection<LogEntry> FilteredEntries { get; }
    
    public LogFilter Filter { get; private set; }

    private CancellationTokenSource? _filterCts;

    private static readonly TimeSpan FilterDebounce = TimeSpan.FromMilliseconds(150);
    
    [ObservableProperty]
    public partial bool IsFiltering { get; set; }

    [ObservableProperty] 
    public partial string? SearchText { get; set; }

    [ObservableProperty] 
    public partial LogLevel Level { get; set; } = LogLevel.Information;
    
    public IReadOnlyList<LogLevel> AvailableLogLevelOptions { get; }

    public NameFilter InstanceFilter { get; } = new();
    
    public NameFilter SourceFilter { get; } = new();
    
    public LogsPageViewModel(ILogStoreReader store,
        LogSettings logSettings,
        ILogService logService,
        IInstanceConfigProvider instanceConfigProvider,
        [FromKeyedServices(NotificationCounterKey.Logs)] INotificationAcknowledger notificationAcknowledger,
        ILogger<LogsPageViewModel> logger)
    {
        _store = store;
        _instanceConfigProvider = instanceConfigProvider;
        _logService = logService;
        _notificationAcknowledger = notificationAcknowledger;
        _logger = logger;
        
        FilteredEntries = new ReadOnlyObservableCollection<LogEntry>(_filteredEntries);

        AvailableLogLevelOptions = Enum.GetValues<LogLevel>()
            .Where(l => LogLevels.IsEnabled(l, logSettings.MinimumLevel))
            .ToArray();

        InstanceFilter.ActiveSelections.CollectionChanged += OnNameFilterActiveSelectionsChanged;
        SourceFilter.ActiveSelections.CollectionChanged += OnNameFilterActiveSelectionsChanged;

        Filter = CreateFilter();
    }

    private LogFilter CreateFilter() =>
        new(Level,
            InstanceFilter.ActiveNames,
            InstanceFilter.IsNoNameActive,
            SourceFilter.ActiveNames,
            SourceFilter.IsNoNameActive,
            SearchText);

    private void TrackSourceModule(LogEntry entry)
    {
        if (!string.IsNullOrEmpty(entry.SourceModule))
            SourceFilter.AddName(entry.SourceModule);
    }

    private void TrackInstanceName(LogEntry entry)
    {
        if (!string.IsNullOrEmpty(entry.InstanceName))
            InstanceFilter.AddName(entry.InstanceName);
    }
    
    private void AddEntry(LogEntry entry)
    {
        _entries.Add(entry);
        
        if (_logService.IsEntryVisible(entry, Filter))
            _filteredEntries.Add(entry);
        
        TrackSourceModule(entry);
        TrackInstanceName(entry);
    }
    
    private void OnNextEntry(LogEntry entry, int generation)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (generation == _subscriptionGeneration)
                AddEntry(entry);
        });
    }

    private void SetFilteredEntries(IEnumerable<LogEntry> entries)
    {
        using var _ = _filteredEntries.SuspendNotifications();
        
        _filteredEntries.Clear();
        _filteredEntries.AddRange(entries);
    }
    
    private async Task UpdateFilterAsync(bool debounce = false)
    {
        // ReSharper disable once MethodHasAsyncOverload
        _filterCts?.Cancel();
        
        var cts = new CancellationTokenSource();
        _filterCts = cts;
        var token = cts.Token;

        try
        {
            if (debounce)
                await Task.Delay(FilterDebounce, token);

            IsFiltering = true;

            var filter = CreateFilter();
            var snapshot = _entries.ToArray();
            var filteredEntries = await Task.Run(
                () => _logService.Filter(snapshot, filter, token),
                token);

            token.ThrowIfCancellationRequested();

            List<LogEntry> extraEntries = [];
            for (var i = snapshot.Length; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (_logService.IsEntryVisible(entry, filter))
                    extraEntries.Add(entry);
            }

            var finalFilteredEntries = extraEntries.Count > 0 
                ? filteredEntries.Concat(extraEntries)
                : filteredEntries;
            
            Filter = filter;
            SetFilteredEntries(finalFilteredEntries);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        finally
        {
            if (ReferenceEquals(_filterCts, cts))
            {
                _filterCts = null;
                IsFiltering = false;
            }
            
            cts.Dispose();
        }
    }

    private async Task UpdateFilterSafeAsync(bool debounce = false)
    {
        try
        {
            await UpdateFilterAsync(debounce);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update filter");
        }
    }

    partial void OnLevelChanged(LogLevel value) => _ = UpdateFilterSafeAsync();

    partial void OnSearchTextChanged(string? value) => _ = UpdateFilterSafeAsync(true);
    
    private void OnNameFilterActiveSelectionsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        _ = UpdateFilterSafeAsync();

    private void SubscribeAndFetchLogs()
    {
        _subscriptionHandle?.Dispose();
        
        var generation = ++_subscriptionGeneration;
        
        var (snapshot, handle) = _store.Subscribe(
            entry => OnNextEntry(entry, generation));
        
        _subscriptionHandle = handle;
        
        _filterCts?.Cancel();
        
        _entries.Clear();
        foreach (var entry in snapshot)
        {
            _entries.Add(entry);
            TrackSourceModule(entry);
            TrackInstanceName(entry);
        }

        _ = UpdateFilterSafeAsync();
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
        FillInstancesFilter();
        
        SubscribeAndFetchLogs();
        
        _notificationAcknowledger.SetAcknowledgingNotifications(true);
        
        return Task.CompletedTask;
    }

    public override Task OnLeaveAsync()
    {
        _notificationAcknowledger.SetAcknowledgingNotifications(false);
        
        _subscriptionHandle?.Dispose();
        _subscriptionHandle = null;
        
        _filterCts?.Cancel();
        
        _entries.Clear();
        _filteredEntries.Clear();
        
        return Task.CompletedTask;
    }
}