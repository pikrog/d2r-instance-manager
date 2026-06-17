using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Agents;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Effects;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.SessionOperations;
using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Engine;

public class GameInstanceEngine : IGameInstanceEngineControl
{
    public Guid Id { get; }
    
    private readonly Channel<Event> _eventChannel = Channel.CreateUnbounded<Event>();
    
    private Session _session = new();
    
    private readonly CancellationTokenSource _engineCancellationTokenSource = new();
    
    private readonly CancellationTokenSource _sessionCancellationTokenSource = new();
    
    private readonly List<Task> _tasks = []; // todo: await all
    
    private readonly LaunchCoordinator _launchCoordinator;
    
    private readonly ProcessStartInfoFactory _processStartInfoFactory;
    
    public event EventHandler<RuntimeSnapshot>? StateChanged;

    private RuntimeSnapshot _runtimeSnapshot;
    
    public RuntimeSnapshot RuntimeSnapshot
    {
        get => Volatile.Read(ref _runtimeSnapshot);
        private set
        {
            Volatile.Write(ref _runtimeSnapshot, value);
            StateChanged?.Invoke(this, value);
        }
    }

    public GameInstanceEngine(Guid id, LaunchCoordinator launchCoordinator, ProcessStartInfoFactory processStartInfoFactory)
    {
        Id = id;
        
        _launchCoordinator = launchCoordinator;
        _processStartInfoFactory = processStartInfoFactory;
        
        _runtimeSnapshot = Snap();
        
        _ = Loop();
    }

    private async Task PublishAsync(Event @event)
    {
        await _eventChannel.Writer.WriteAsync(@event, _engineCancellationTokenSource.Token);
    }
    
    public async Task StartAsync(LaunchContext launchContext)
    {
        var @event = new StartRequested(launchContext);
        await PublishAsync(@event);
    }
    
    public async Task StopAsync()
    {
        await _sessionCancellationTokenSource.CancelAsync();
    }

    public async Task ShutdownAsync()
    {
        await _engineCancellationTokenSource.CancelAsync();
        // todo: await _loopTask;
    }

    private TransitionResult ApplyTransition(Event @event)
    { 
        // todo: separate session effects (DisposeLaunchLease) and pure side-effects (UnlockMultibox)?
        switch (_session.State, @event)
        {
            case (State.Inactive, StartRequested e): // todo: effect: await sessionToken.CancelAsync() + await effect tasks + flush event queue
                switch (e.LaunchContext.AuthenticationContext)
                {
                    case OsiAuthenticationContext c:
                        return new TransitionResult(State.Authenticating, [new Authenticate(e.LaunchContext.AuthenticationContext)], [new CreateSession(), new SetProcessStartInfo(_processStartInfoFactory.Create(e.LaunchContext))]);
                    case OfflineAuthenticationContext or CliAuthenticationContext:
                        return new TransitionResult(State.WaitingForStart, [new AcquireLaunchLease(_launchCoordinator)], [new CreateSession(), new SetProcessStartInfo(_processStartInfoFactory.Create(e.LaunchContext)), new SetLaunchLeaseRequested()]);
                    default:
                        throw new InvalidOperationException($"Unexpected authentication context: {e.LaunchContext.AuthenticationContext.GetType().Name}");
                }
            
            case (State.Authenticating, Authenticated):
                return new TransitionResult(State.WaitingForStart, [new AcquireLaunchLease(_launchCoordinator)], [new SetLaunchLeaseRequested()]);
            case (State.Authenticating, AuthenticationFailed):
            case (State.Authenticating, StopRequested):
                return new TransitionResult(State.Stopping, [new CheckCleanupCompletion(_session.CleanupState)]);
            
            case (State.WaitingForStart, LaunchLeaseGranted e):
                return new TransitionResult(State.Starting, [new StartProcess(_session.ProcessStartInfo!)], [new SetLease(e.Lease), new SetProcessStartRequested()]);
            case (State.WaitingForStart, LaunchLeaseCanceled):
                return new TransitionResult(State.Stopping, [], [new SetLaunchLeaseCleanedUp()]);
            case (State.WaitingForStart, StopRequested):
                return new TransitionResult(State.Stopping, [new CheckCleanupCompletion(_session.CleanupState)]);
            
            case (State.Starting, ProcessStarted e):
                return new TransitionResult(State.WaitingForUnlock, [new MonitorProcessExit(e.Process), new UnlockMultibox()], [new SetProcess(e.Process)]);
            case (State.Starting, ProcessStartFailed e):
                return new TransitionResult(State.Stopping, [new ReleaseLaunchLease(_session.Lease!)], [new SetProcessCleanedUp()]);
            case (State.Starting, StopRequested):
                return new TransitionResult(State.Stopping, [new ReleaseLaunchLease(_session.Lease!)]);
            
            case (State.WaitingForUnlock, MultiboxUnlocked):
                return new TransitionResult(State.Running, [new ReleaseLaunchLease(_session.Lease!)]);
            case (State.WaitingForUnlock, MultiboxUnlockFailed):
            case (State.WaitingForUnlock, StopRequested):
                return new TransitionResult(State.Stopping, [new ReleaseLaunchLease(_session.Lease!), new StopProcess(_session.Process!)]);
            case (State.WaitingForUnlock, ProcessExited):
                return new TransitionResult(State.Stopping, [new ReleaseLaunchLease(_session.Lease!)], [new SetProcessCleanedUp()]);
            
            case (State.Running, LaunchLeaseReleased):
                return new TransitionResult(State.Running, [], [new SetLaunchLeaseCleanedUp()]);
            case (State.Running, StopRequested):
                return new TransitionResult(State.Stopping, [new StopProcess(_session.Process!)]);
            case (State.Running, ProcessExited):
                return new TransitionResult(State.Stopping, [new CheckCleanupCompletion(_session.CleanupState)], [new SetProcessCleanedUp()]);
            
            case (State.Stopping, ProcessStarted e):
                return new TransitionResult(State.Stopping, [new MonitorProcessExit(e.Process), new StopProcess(e.Process)]);
            case (State.Stopping, LaunchLeaseGranted e):
                return new TransitionResult(State.Stopping, [new ReleaseLaunchLease(e.Lease)]);
            case (State.Stopping, ProcessExited):
            case (State.Stopping, ProcessStartFailed):
                return new TransitionResult(State.Stopping, [new CheckCleanupCompletion(_session.CleanupState)], [new SetProcessCleanedUp()]);
            case (State.Stopping, LaunchLeaseReleased):
                return new TransitionResult(State.Stopping, [new CheckCleanupCompletion(_session.CleanupState)], [new SetLaunchLeaseCleanedUp()]);
            case (State.Stopping, CleanupCompleted):
                return new TransitionResult(State.Inactive);
            
            default:
                return new TransitionResult(_session.State);
        }
    }

    private async Task Loop()
    {
        var engineCancellationToken = _engineCancellationTokenSource.Token;
        try
        {
            while (await _eventChannel.Reader.WaitToReadAsync(engineCancellationToken).ConfigureAwait(false))
            {
                await foreach (var @event in _eventChannel.Reader.ReadAllAsync(engineCancellationToken).ConfigureAwait(false))
                {
                    await HandleEvent(@event);
                    RuntimeSnapshot = Snap();
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            // todo: Log
            Console.Error.WriteLine(e);
        }
        finally
        {
            _session.Lease?.Dispose();
            _session.Process?.Dispose();
            
            _engineCancellationTokenSource.Dispose(); // todo: move to Dispose
            _sessionCancellationTokenSource.Dispose();
        }
    }
    
    private async Task HandleEvent(Event @event) // todo: pure
    {
        // todo: pure functions ApplyTransition, ExecuteEffect, ExecuteSessionOperation
        var (nextState, effects, sessionOperations) = ApplyTransition(@event);
        UpdateSession(nextState, sessionOperations, @event as ErrorEvent);
        foreach (var effect in effects)
            await ExecuteEffect(effect);
    }

    private void UpdateSession(State nextState, IReadOnlyList<SessionOperation> sessionOperations, ErrorEvent? errorEvent)
    {
        _session = _session with { State = nextState };
        foreach (var operation in sessionOperations)
            _session = ExecuteSessionOperation(operation);
        if (errorEvent is not null)
            _session = _session with { ErrorEvents = _session.ErrorEvents.Add(errorEvent) };
    }

    private Session ExecuteSessionOperation(SessionOperation operation) =>
        operation switch
        {
            CreateSession => new Session(),
            SetProcessStartInfo info => _session with { ProcessStartInfo = info.ProcessStartInfo },
            SetProcess process => _session with { Process = process.Process },
            SetLease lease => _session with { Lease = lease.Lease },
            SetLaunchLeaseRequested => _session with { CleanupState = _session.CleanupState with { IsLeaseRequested = true } },
            SetProcessStartRequested => _session with { CleanupState = _session.CleanupState with { IsProcessStartRequested = true } },
            SetLaunchLeaseCleanedUp => _session with { CleanupState = _session.CleanupState with { IsLeaseCleanedUp = true } },
            SetProcessCleanedUp => _session with { CleanupState = _session.CleanupState with { IsProcessCleanedUp = true } },
            _ => throw new InvalidOperationException($"Unexpected session operation: {operation.GetType().Name}")
        };


    private async Task ExecuteEffect(Effect effect)
    {
        switch (effect)
        {
            case Authenticate e:
                RunSessionAgent(new AuthenticateAgent(e.AuthenticationContext));
                break;
            case AcquireLaunchLease e:
                RunSessionAgent(new AcquireLaunchLeaseAgent(e.LaunchCoordinator));
                break;
            case ReleaseLaunchLease e:
                e.Lease.Dispose();
                await PublishAsync(new LaunchLeaseReleased());
                break;
            case StartProcess e:
                RunSessionAgent(new StartProcessAgent(e.ProcessStartInfo));
                break;
            case UnlockMultibox _:
                RunSessionAgent(new UnlockMultiboxAgent(new RetryingMultiboxUnlocker(new RetryPolicy(TimeSpan.FromSeconds(1), 3)))); // todo: read policy from event
                break;
            case MonitorProcessExit e:
                RunCleanupAgent(new MonitorProcessExitAgent(e.Process));
                break;
            case StopProcess e:
                RunCleanupAgent(new StopProcessAgent(e.Process, new RetryingProcessStopper(new RetryPolicy(TimeSpan.FromSeconds(1), 3), TimeSpan.FromSeconds(10))));
                break;
            case CheckCleanupCompletion e:
                if (e.CleanupState.IsCleanupComplete)
                    await PublishAsync(new CleanupCompleted());
                break;
            default:
                throw new InvalidOperationException($"Unexpected effect: {effect.GetType().Name}");
        }
    }

    private void RunSessionAgent(IGameInstanceEngineAgent agent) => RunAgent(agent, _sessionCancellationTokenSource.Token);

    private void RunCleanupAgent(IGameInstanceEngineAgent agent) => RunAgent(agent, _engineCancellationTokenSource.Token);

    private void RunAgent(IGameInstanceEngineAgent agent, CancellationToken cancellationToken)
    {
        var task = RunAgentAndPublishEvent(agent, cancellationToken);
        _tasks.Add(task);
    }

    private async Task RunAgentAndPublishEvent(IGameInstanceEngineAgent agent, CancellationToken cancellationToken)
    {
        try
        {
            var result = await agent.RunAsync(cancellationToken);
            if (result is not null)
                await PublishAsync(result);
        }
        catch (Exception e)
        {
            var error = new UnknownError(e, agent.GetType().Name);
            await PublishAsync(error);
        }
    }

    private RuntimeSnapshot Snap() => new(Id, _session.State, _session.ErrorEvents);
}



