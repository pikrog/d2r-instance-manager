using System;

namespace AvaloniaApplication1.Region.Models;

public record RegionSummary(Guid Id, string Name, string Address, int InstanceCount);