using System;

namespace AvaloniaApplication1.ViewModels;

public interface IDialogAware
{
    public Action<bool>? CloseDialog { get; set; }
}