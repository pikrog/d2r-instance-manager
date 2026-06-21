using System;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record UnexpectedError(Exception Exception, string TaskName) : ErrorEvent;