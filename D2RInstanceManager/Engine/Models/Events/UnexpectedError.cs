using System;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record UnexpectedError(Exception Exception, string TaskName) : ErrorEvent
{
    public override string Message => Exception.Message;
}