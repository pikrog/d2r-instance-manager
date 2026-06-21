using System;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts.Helpers;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record StopProcess(Process Process, ProcessStopPolicies Policies) : Effect;