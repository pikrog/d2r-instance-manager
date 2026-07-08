using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

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