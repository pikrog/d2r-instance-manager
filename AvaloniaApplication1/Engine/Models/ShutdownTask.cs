using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine.Models;

public record ShutdownTask(Task Task, bool TrackProgress);