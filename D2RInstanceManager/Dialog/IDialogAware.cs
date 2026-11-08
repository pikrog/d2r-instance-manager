using System;

namespace D2RInstanceManager.Dialog;

public interface IDialogAware
{
    public Action<bool>? CloseDialog { get; set; }
}