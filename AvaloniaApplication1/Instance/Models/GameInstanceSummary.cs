using System;
using System.Collections.Generic;
using AvaloniaApplication1.Instance.Models.Issues;

namespace AvaloniaApplication1.Instance.Models;

public record GameInstanceSummary(Guid Id, string Name, GameInstanceStatus Status, bool IsActive, IReadOnlyList<GameInstanceIssue> Issues);