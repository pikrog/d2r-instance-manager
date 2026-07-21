using System;

namespace AvaloniaApplication1.Instance.Models;

public record GameInstanceSummary(Guid Id, string Name, GameInstanceStatus Status, bool IsActive);