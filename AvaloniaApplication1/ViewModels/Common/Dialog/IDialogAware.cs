using System;

namespace AvaloniaApplication1.ViewModels.Common.Dialog;

public interface IDialogAware
{
    public Action<bool>? CloseDialog { get; set; }
}