using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Agents;
using AvaloniaApplication1.Engine.Coordination;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Engine.Helpers.MultiboxUnlock;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Effects;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Engine;

public class InstanceEngine : IAsyncDisposable
{
    public Guid Id { get; }
    
    private readonly Channel<Event> _eventChannel = Channel.CreateUnbounded<Event>();
    
    private Session _session = new();
    
    private readonly CancellationTokenSource _engineCancellationTokenSource = new();

    private readonly Task _loopTask;
    
    private CancellationTokenSource _sessionCancellationTokenSource = new();
    
    private readonly List<Task> _sessionTasks = [];
    
    private readonly LaunchCoordinator _launchCoordinator;
    
    private readonly ProcessStartInfoFactory _processStartInfoFactory;
    
    public event EventHandler<Guid>? StateChanged;

    private RuntimeSnapshot _runtimeSnapshot;
    
    public RuntimeSnapshot RuntimeSnapshot
    {
        get => Volatile.Read(ref _runtimeSnapshot);
        private set
        {
            Volatile.Write(ref _runtimeSnapshot, value);
            StateChanged?.Invoke(this, Id);
        }
    }

    public InstanceEngine(Guid id, LaunchCoordinator launchCoordinator, ProcessStartInfoFactory processStartInfoFactory)
    {
        Id = id;
        
        _launchCoordinator = launchCoordinator;
        _processStartInfoFactory = processStartInfoFactory;
        
        _runtimeSnapshot = Snap();
        
        _loopTask = Loop();
    }

    private async Task PublishAsync(Event @event)
    {
        await _eventChannel.Writer.WriteAsync(@event, _engineCancellationTokenSource.Token);
    }
    
    public async Task LaunchAsync(EngineLaunchContext context)
    {
        var processStartInfo = _processStartInfoFactory.Create(context.InstanceLaunchContext);
        var @event = new LaunchRequested(context.InstanceLaunchContext.AuthenticationContext, processStartInfo, context.Policies);
        await PublishAsync(@event);
    }
    
    public async Task StopAsync()
    {
        var @event = new StopRequested();
        await PublishAsync(@event);
    }

    public async Task GracefulShutdownAsync()
    {
        await PublishAsync(new ShutdownRequested());
        await _loopTask;
        _sessionCancellationTokenSource.Dispose();
        _engineCancellationTokenSource.Dispose();
    }

    public async Task ShutdownAsync()
    {
        await _engineCancellationTokenSource.CancelAsync();
        await _loopTask;
        await _sessionCancellationTokenSource.CancelAsync();
        await Task.WhenAll(_sessionTasks);
        _session.Dispose();
        _sessionCancellationTokenSource.Dispose();
        _engineCancellationTokenSource.Dispose();
    }

    private async Task Loop()
    {
        var engineCancellationToken = _engineCancellationTokenSource.Token;
        try
        {
            while (await _eventChannel.Reader.WaitToReadAsync(engineCancellationToken).ConfigureAwait(false))
            {
                await foreach (var @event in _eventChannel.Reader.ReadAllAsync(engineCancellationToken)
                                   .ConfigureAwait(false))
                {
                    await HandleEvent(@event);
                    RuntimeSnapshot = Snap();
                    if (_session.State == State.Shutdown)
                        return;
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            _session = _session.AddErrorEvent(new UnexpectedError(e, nameof(Loop))) with { State = State.Inactive };
            RuntimeSnapshot = Snap();
        }
    }
    
    private async Task HandleEvent(Event @event)
    {
        var previousState = _session.State;
        var result = InstanceStateMachine.Apply(_session, @event);

        Debug.WriteLine($"[{Id}] <state> ({@event.GetType().Name}) {previousState}->{result.Session.State}");
        _session = result.Session;

        foreach (var effect in result.Effects)
        {
            Debug.WriteLine($"[{Id}] <fx> {effect.GetType().Name}");
            await ExecuteEffect(effect);
        }
    }

    private async Task ExecuteEffect(Effect effect)
    {
        switch (effect)
        {
            case Authenticate e:
                RunSessionAgent(new AuthenticateAgent(e.AuthenticationContext));
                break;
            case AcquireLaunchLease:
                RunSessionAgent(new AcquireLaunchLeaseAgent(_launchCoordinator));
                break;
            case ReleaseLaunchLease e:
                e.Lease.Dispose();
                await PublishAsync(new LaunchLeaseReleased());
                break;
            case StartProcess e:
                RunSessionAgent(new StartProcessAgent(e.ProcessStartInfo));
                break;
            case UnlockMultibox e:
                RunSessionAgent(new UnlockMultiboxAgent(new RetryingMultiboxUnlocker(e.RetryPolicy)));
                break;
            case MonitorProcessExit e:
                RunCleanupAgent(new MonitorProcessExitAgent(e.ProcessManager, e.ForcefulExitCode));
                break;
            case StopProcess e:
                RunCleanupAgent(new StopProcessAgent(e.ProcessManager, new RetryingProcessStopper(e.Policies)));
                break;
            case Cancel:
                await _sessionCancellationTokenSource.CancelAsync();
                break;
            case Reset:
                await Task.WhenAll(_sessionTasks);
                _sessionTasks.Clear();
                _sessionCancellationTokenSource.Dispose();
                _sessionCancellationTokenSource = new CancellationTokenSource();
                _session.Dispose();
                break;
            default:
                throw new InvalidOperationException($"Unexpected effect: {effect.GetType().Name}");
        }
    }

    private void RunSessionAgent(IAgent agent) => RunAgent(agent, _sessionCancellationTokenSource.Token);

    private void RunCleanupAgent(IAgent agent) => RunAgent(agent, _engineCancellationTokenSource.Token);

    private void RunAgent(IAgent agent, CancellationToken cancellationToken)
    {
        var task = RunAgentAndPublishEvent(agent, cancellationToken);
        _sessionTasks.Add(task);
    }

    private async Task RunAgentAndPublishEvent(IAgent agent, CancellationToken cancellationToken)
    {
        try
        {
            var result = await agent.RunAsync(cancellationToken);
            if (result is not null)
                await PublishAsync(result);
        }
        catch (Exception e)
        {
            var error = new UnexpectedError(e, agent.GetType().Name);
            await PublishAsync(error);
        }
    }

    private RuntimeSnapshot Snap() =>
        new(Id, _session.State, _session.Process, _session.ProcessExitResult, _session.ErrorEvents);

    public async ValueTask DisposeAsync()
    {
        await ShutdownAsync();
    }
}
