using System;
using D2RInstanceManager.HotKey.Coordination;

namespace D2RInstanceManager.Instance.Models;

public sealed record ShowInstanceHotKeyCommand(Guid InstanceId) : HotKeyCommand;