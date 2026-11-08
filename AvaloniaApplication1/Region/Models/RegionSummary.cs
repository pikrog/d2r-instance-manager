using System;

namespace D2RInstanceManager.Region.Models;

public record RegionSummary(Guid Id, string Name, string Address, int InstanceCount);