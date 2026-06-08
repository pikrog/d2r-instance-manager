using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public class GameInstanceAlreadyExistsException(Guid id)
    : GameInstanceException($"Game instance with id {id} already exists");