using System;
using System.Collections.Generic;
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
using AvaloniaApplication1.Engine.Models.Messages;
using AvaloniaApplication1.Engine.Models.StateMachine;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Engine;

public class InstanceEngine : IAsyncDisposable
{
    public const string IdLogProperty = "InstanceId";
    
    public Guid Id { get; }
    
    private readonly Channel<Message> _channel = Channel.CreateUnbounded<Message>();
    
    private Session _session = new();
    
    private readonly CancellationTokenSource _engineCancellationTokenSource = new();

    private readonly Task _loopTask;
    
    private CancellationTokenSource _sessionCancellationTokenSource = new();
    
    private readonly List<Task> _sessionTasks = [];
    
    private readonly LaunchCoordinator _launchCoordinator;
    
    private readonly ProcessStartRequestFactory _processStartRequestFactory;
    
    private readonly ILoggerFactory _loggerFactory;
    
    private readonly ILogger<InstanceEngine> _logger;

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

    public InstanceEngine(Guid id, LaunchCoordinator launchCoordinator, ProcessStartRequestFactory processStartRequestFactory, ILoggerFactory loggerFactory)
    {
        Id = id;
        
        _launchCoordinator = launchCoordinator;
        _processStartRequestFactory = processStartRequestFactory;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<InstanceEngine>();
        
        _runtimeSnapshot = Snap();
        
        _loopTask = Loop();
    }
    
    private ValueTask WriteChannelAsync(Message message) => 
        _channel.Writer.WriteAsync(message, _engineCancellationTokenSource.Token);

    private ValueTask PublishAsync(Event @event)
    {
        var message = new EventMessage(@event);
        return WriteChannelAsync(message);
    }

    private async Task PublishCompletableAsync(Event @event)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var message = new CompletableEventMessage(@event, completion);
        await WriteChannelAsync(message);
        await completion.Task;
    }
    
    public Task LaunchAsync(EngineLaunchContext context)
    {
        var processStartContext = ProcessStartContextFactory.Create(context.InstanceLaunchContext);
        var @event = new LaunchRequested(context.InstanceLaunchContext.AuthenticationContext, processStartContext, context.Policies);
        return PublishCompletableAsync(@event);
    }
    
    public Task StopAsync()
    {
        var @event = new StopRequested();
        return PublishCompletableAsync(@event);
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

    public async Task FlushAsync() // todo: what if channel is complete?
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var message = new FlushRequested(completion);
        await WriteChannelAsync(message);
        await completion.Task;
    }

    private async Task HandleMessage(Message message)
    {
        try
        {
            switch (message)
            {
                case EventMessage eventMessage:
                    await HandleEvent(eventMessage.Event);
                    break;
                case FlushRequested:
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected message: {message.GetType().Name}");
            }

            RuntimeSnapshot = Snap();
            
            if (message is ICompletableMessage completableMessage)
                completableMessage.Completion.SetResult();
        }
        catch (Exception e)
        {
            if (message is ICompletableMessage completableMessage)
                completableMessage.Completion.SetException(e);

            throw;
        }
    }

    private async Task Loop() // todo: loop restart or move lifecycle management to manager
    {
        using var _ = _logger.BeginScope(new Dictionary<string, object> {{IdLogProperty, Id}});
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(_engineCancellationTokenSource.Token))
            {
                await HandleMessage(message);
                if (_session.State == State.Shutdown)
                    return;
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error in engine loop");
            
            _session = _session.AddErrorEvent(new UnexpectedError(e, nameof(Loop))) with { State = State.Inactive };
            RuntimeSnapshot = Snap();
        }
        finally
        {
            _channel.Writer.TryComplete();

            // Deliberately uncancellable: drain all pending messages after engine cancellation.
            // ReSharper disable MethodSupportsCancellation
            await foreach (var message in _channel.Reader.ReadAllAsync())
            {
                if (message is ICompletableMessage completableMessage)
                    completableMessage.Completion.SetCanceled();
            }
            // ReSharper restore MethodSupportsCancellation
        }
    }
    
    private async Task HandleEvent(Event @event)
    {
        var previousState = _session.State;
        var result = InstanceStateMachine.Apply(_session, @event);
        
        _logger.LogTrace(
            "State transition: {PreviousState}->{NextState} (caused by event: {Event})", 
            previousState, result.Session.State, @event);
        
        _session = result.Session;

        foreach (var effect in result.Effects)
        {
            _logger.LogTrace("Executing effect: {Effect}", effect);
            
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
                RunSessionAgent(
                    new StartProcessAgent(
                        e.ProcessStartContext, 
                        _processStartRequestFactory,
                        _loggerFactory.CreateLogger<StartProcessAgent>()));
                break;
            case UnlockMultibox e:
                RunSessionAgent(
                    new UnlockMultiboxAgent(
                        new RetryingMultiboxUnlocker(e.RetryPolicy), 
                        e.ExpectedProcessId, 
                        _loggerFactory.CreateLogger<UnlockMultiboxAgent>()));
                break;
            case MonitorProcessExit e:
                RunCleanupAgent(
                    new MonitorProcessExitAgent(
                        e.ProcessManager, 
                        e.ForcefulExitCode, 
                        _loggerFactory.CreateLogger<MonitorProcessExitAgent>()));
                break;
            case StopProcess e:
                RunCleanupAgent(
                    new StopProcessAgent(
                        e.ProcessManager, 
                        new RetryingProcessStopper(e.Policies),
                        _loggerFactory.CreateLogger<StopProcessAgent>()));
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
