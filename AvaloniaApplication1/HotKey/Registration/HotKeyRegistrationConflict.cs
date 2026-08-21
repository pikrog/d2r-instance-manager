namespace AvaloniaApplication1.HotKey.Registration;

public sealed record HotKeyRegistrationConflict(KeyCombination KeyCombination, HotKeyConflictSource Source) : HotKeyRegistration;