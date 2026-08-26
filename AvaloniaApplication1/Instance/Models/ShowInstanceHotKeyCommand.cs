using System;
using AvaloniaApplication1.HotKey.Coordination;

namespace AvaloniaApplication1.Instance.Models;

public sealed record ShowInstanceHotKeyCommand(Guid InstanceId) : HotKeyCommand;