using System;
using System.Collections.Generic;
using D2RInstanceManager.Instance.Models.Issues;

namespace D2RInstanceManager.Instance.Models;

public record InstanceSummary(Guid Id, string Name, InstanceStatus Status, bool IsActive, IReadOnlyList<InstanceIssue> Issues);