using System;

namespace D2RInstanceManager.Engine.Exceptions;

public class InstanceNotFoundException(Guid id) : InstanceException($"Game instance with id {id} not found");