using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Events.Authentication;

namespace AvaloniaApplication1.Engine.Agents;

public enum AuthenticationResult
{
    Success,
    Failure,
}

public class AuthenticateAgent(AuthenticationContext context) : GameInstanceEngineAgentBase<AuthenticationResult>
{
    protected override Task<AuthenticationResult> RunAgentTaskAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException("AuthenticateAgent not implemented yet");

    protected override Event MapAgentResultToEvent(AuthenticationResult result) =>
        new AuthenticationFailed();
}