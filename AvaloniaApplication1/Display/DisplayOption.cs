namespace AvaloniaApplication1.Display;

public abstract record DisplayOption
{
    public sealed record Primary : DisplayOption
    {
        public static readonly Primary Instance = new();
    }

    public sealed record Specific(int? Index, DisplaySnapshot Snapshot, bool IsConnected) : DisplayOption
    {
        public string Id => Snapshot.Id;
        
        public string Label => $"{(Index is not null ? $"{Index} - " : "")}{Snapshot.Description} ({Snapshot.Width}x{Snapshot.Height}){(IsConnected ? "" : " [disconnected]")}";
    }

    public static readonly DisplayOption Default = Primary.Instance;
};