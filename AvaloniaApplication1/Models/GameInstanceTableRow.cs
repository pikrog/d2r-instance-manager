using System;
using AvaloniaApplication1.Engine.Models;

namespace AvaloniaApplication1.Models;

public record GameInstanceTableRow(Guid Id, string Name, GameInstanceStatus Status);