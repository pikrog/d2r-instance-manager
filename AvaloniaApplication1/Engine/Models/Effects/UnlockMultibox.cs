using AvaloniaApplication1.Engine.Models.Common;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record UnlockMultibox(RetryPolicy RetryPolicy) : Effect;