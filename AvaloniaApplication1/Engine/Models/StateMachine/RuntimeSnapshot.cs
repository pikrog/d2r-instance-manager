using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Results;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record RuntimeSnapshot(Guid Id, State State, uint? ExitCode, ProcessStopMode? StopMode, ImmutableArray<ErrorEvent> Errors);