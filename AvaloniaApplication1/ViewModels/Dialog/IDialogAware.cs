using System;

namespace AvaloniaApplication1.ViewModels.Dialog;

public interface IDialogAware
{
    public Action<bool>? CloseDialog { get; set; }
}