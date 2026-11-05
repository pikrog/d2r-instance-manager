namespace AvaloniaApplication1.Overlay.Dialog.SetPassphrase;

public abstract record SetPassphraseResult
{
    public sealed record Canceled : SetPassphraseResult
    {
        public static readonly Canceled Instance = new();
    }

    public sealed record Entered(string Passphrase) : SetPassphraseResult;
};