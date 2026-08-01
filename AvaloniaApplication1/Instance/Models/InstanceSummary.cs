using System;
using System.Collections.Generic;
using AvaloniaApplication1.Instance.Models.Issues;

namespace AvaloniaApplication1.Instance.Models;

public record InstanceSummary(Guid Id, string Name, InstanceStatus Status, bool IsActive, IReadOnlyList<InstanceIssue> Issues);