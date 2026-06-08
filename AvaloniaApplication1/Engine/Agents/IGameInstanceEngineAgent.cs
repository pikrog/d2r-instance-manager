using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

public interface IGameInstanceEngineAgent
{
    Task<Event?> RunAsync(CancellationToken cancellationToken);
}