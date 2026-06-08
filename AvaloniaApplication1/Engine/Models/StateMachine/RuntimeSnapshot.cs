using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record RuntimeSnapshot(Guid Id, State State, ImmutableArray<ErrorEvent> Errors);