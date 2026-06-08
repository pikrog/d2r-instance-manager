using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

public abstract class GameInstanceEngineAgentBase<TResult> : IGameInstanceEngineAgent
{
    protected abstract Task<TResult> RunAgentTaskAsync(CancellationToken cancellationToken);
    
    protected abstract Event MapAgentResultToEvent(TResult result);
    
    protected virtual ErrorEvent CreateErrorForGenericException(Exception exception) => new UnknownError(exception, GetType().Name);

    protected virtual Event? CreateCanceledEvent() => null;
    
    public async Task<Event?> RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await RunAgentTaskAsync(cancellationToken);
            return MapAgentResultToEvent(result);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return CreateCanceledEvent();
        }
        catch (Exception e)
        {
            return CreateErrorForGenericException(e);
        }
    }
}

/*
public abstract class GameInstanceEngineAgentBase : IGameInstanceEngineAgent
{
    protected abstract Task RunAgentTaskAsync(CancellationToken cancellationToken);
    
    protected abstract Event CreateSuccessEvent();
    
    protected virtual ErrorEvent CreateErrorForGenericException(Exception exception) => new UnknownError(exception, GetType().Name);
    
    protected virtual Event? CreateCanceledEvent() => null;
    
    public async Task<Event?> RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await RunAgentTaskAsync(cancellationToken);
            return CreateSuccessEvent();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return CreateCanceledEvent();
        }
        catch (Exception e)
        {
            return CreateErrorForGenericException(e);
        }
    }
}
*/
