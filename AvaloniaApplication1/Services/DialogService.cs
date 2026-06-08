using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AvaloniaApplication1.Services;

public class DialogService
{
    #region RegisterProperty
    public static readonly AttachedProperty<IDialogParticipant?> RegisterProperty 
        = AvaloniaProperty.RegisterAttached<DialogService, Visual, IDialogParticipant?>("Register");
    
    public static IDialogParticipant? GetRegister(AvaloniaObject @object, DialogService dialogService)
    {
        return @object.GetValue(RegisterProperty);
    }

    public static void SetRegister(AvaloniaObject @object, IDialogParticipant value)
    {
        @object.SetValue(RegisterProperty, value);
    }
    #endregion

    private static readonly Dictionary<IDialogParticipant, Visual> RegistrationMapper = [];
    
    static DialogService()
    {
        RegisterProperty.Changed.AddClassHandler<Visual>(RegisterChanged);
    }
    
    private static void RegisterChanged(Visual visual, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.GetOldValue<IDialogParticipant>() is { } previousParticipant)
        {
            RegistrationMapper.Remove(previousParticipant);
        }

        if (e.GetNewValue<IDialogParticipant>() is { } newParticipant)
        {
            RegistrationMapper.Add(newParticipant, visual);
        }
    }

    public static Visual? GetVisual(IDialogParticipant participant)
    {
        return RegistrationMapper.GetValueOrDefault(participant);
    }

    public static TopLevel? GetTopLevel(IDialogParticipant participant)
    {
        var visual = GetVisual(participant);
        return TopLevel.GetTopLevel(visual);
    }

    public static Window? GetMainWindow(IDialogParticipant participant)
    {
        return GetTopLevel(participant) as Window;
    }
}