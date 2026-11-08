using System.Threading.Tasks;

namespace D2RInstanceManager.Engine.Models;

public record ShutdownRequest(Task Completion, bool TrackProgress);