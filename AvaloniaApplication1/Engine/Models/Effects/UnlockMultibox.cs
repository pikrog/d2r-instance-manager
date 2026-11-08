using D2RInstanceManager.Engine.Common;

namespace D2RInstanceManager.Engine.Models.Effects;

public sealed record UnlockMultibox(RetryPolicy RetryPolicy, uint ExpectedProcessId) : Effect;