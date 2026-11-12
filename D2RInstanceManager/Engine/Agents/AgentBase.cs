using System;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Engine.Agents;

public abstract class AgentBase<TResult> : IAgent
{
    protected abstract Task<TResult> RunAgentTaskAsync(CancellationToken cancellationToken);
    
    protected abstract Event MapAgentResultToEvent(TResult result);
    
    protected virtual void OnGenericException(Exception exception) { }
    
    protected virtual ErrorEvent CreateErrorForGenericException(Exception exception) => throw exception;

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
            OnGenericException(e);
            return CreateErrorForGenericException(e);
        }
    }
}