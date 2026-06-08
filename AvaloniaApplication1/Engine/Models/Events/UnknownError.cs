using System;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record UnknownError(Exception Exception, string TaskName) : ErrorEvent;