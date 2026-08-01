using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public class InstanceNotFoundException(Guid id) : InstanceException($"Game instance with id {id} not found");