using System;

namespace D2RInstanceManager.Engine.Exceptions;

public class InstanceAlreadyExistsException(Guid id)
    : InstanceException($"Game instance with id {id} already exists");