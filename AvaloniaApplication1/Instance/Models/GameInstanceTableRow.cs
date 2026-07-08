using System;

namespace AvaloniaApplication1.Instance.Models;

public record GameInstanceTableRow(Guid Id, string Name, GameInstanceStatus Status, bool IsActive);