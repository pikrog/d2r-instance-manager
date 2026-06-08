using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.SessionOperations;

public record SetProcessStartInfo(ProcessStartInfo ProcessStartInfo) : SessionOperation;