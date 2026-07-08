using System;

namespace AvaloniaApplication1.Dialog;

public interface IDialogAware
{
    public Action<bool>? CloseDialog { get; set; }
}