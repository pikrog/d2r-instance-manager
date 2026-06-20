using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

public interface IAgent
{
    Task<Event?> RunAsync(CancellationToken cancellationToken);
}