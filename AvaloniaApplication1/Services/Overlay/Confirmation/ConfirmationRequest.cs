namespace AvaloniaApplication1.Services.Overlay.Confirmation;

public sealed record ConfirmationRequest(string Title, string Message, string Action) : IOverlayRequest<bool>
{
    public IOverlayContent<bool> CreateContent() => new ConfirmationDialogViewModel(Title, Message, Action);
}