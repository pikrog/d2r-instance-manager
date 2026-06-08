using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record StartProcess(ProcessStartInfo ProcessStartInfo) : Effect;