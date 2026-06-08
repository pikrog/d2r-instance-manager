using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public class GameInstanceNotFoundException(Guid id) : GameInstanceException($"Game instance with id {id} not found");