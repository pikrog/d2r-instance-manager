using AvaloniaApplication1.Engine.Common;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record UnlockMultibox(RetryPolicy RetryPolicy, uint ExpectedProcessId) : Effect;