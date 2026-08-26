namespace AvaloniaApplication1.HotKey.Coordination;

public record HotKeyBinding(KeyCombination KeyCombination, HotKeyCommand Command);

public sealed record HotKeyBinding<T>(KeyCombination KeyCombination, T Command) : HotKeyBinding(KeyCombination, Command)
    where T : HotKeyCommand
{
    public new T Command => (T)base.Command;
}
