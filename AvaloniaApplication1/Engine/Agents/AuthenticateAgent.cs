using System;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Engine.Models.Contexts.Launch;
using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Engine.Agents;

public enum AuthenticationResult
{
    Success,
    Failure,
}

public class AuthenticateAgent(AuthenticationContext context) : AgentBase<AuthenticationResult>
{
    protected override Task<AuthenticationResult> RunAgentTaskAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException("AuthenticateAgent not implemented yet");

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) =>
        new AuthenticationFailed();

    protected override Event MapAgentResultToEvent(AuthenticationResult result) =>
        new AuthenticationFailed();
}