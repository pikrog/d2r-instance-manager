using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public class InstanceAlreadyExistsException(Guid id)
    : InstanceException($"Game instance with id {id} already exists");