using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Controls;

public partial class HotkeyBox : TemplatedControl
{
    #region KeyGesture
    public static readonly DirectProperty<HotkeyBox, KeyGesture> KeyGestureProperty =
        AvaloniaProperty.RegisterDirect<HotkeyBox, KeyGesture>(
            nameof(KeyGesture), 
            c => c.KeyGesture,
            (c, v) => c.KeyGesture = v,
            defaultBindingMode: BindingMode.TwoWay
            );

    public KeyGesture KeyGesture
    {
        get;
        set
        {
            if (!SetAndRaise(KeyGestureProperty, ref field, value))
                return;
            UpdateDisplayText();
        }
    } = new(Key.None);
    #endregion KeyGesture
    
    #region DisplayText
    public static readonly DirectProperty<HotkeyBox, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<HotkeyBox, string>(nameof(DisplayText), c => c.DisplayText);

    public string DisplayText
    {
        get;
        private set => SetAndRaise(DisplayTextProperty, ref field, value);
    } = string.Empty;
    #endregion
    
    #region PlaceholderText
    public static readonly StyledProperty<string> EmptyPlaceholderTextProperty =
        AvaloniaProperty.Register<HotkeyBox, string>(
            nameof(EmptyPlaceholderText),
            defaultBindingMode: BindingMode.OneWay);

    public string EmptyPlaceholderText
    {
        get => GetValue(EmptyPlaceholderTextProperty);
        set => SetValue(EmptyPlaceholderTextProperty, value);
    }

    public static readonly StyledProperty<string> CapturingPlaceholderTextProperty =
        AvaloniaProperty.Register<HotkeyBox, string>(
            nameof(CapturingPlaceholderText), 
            defaultBindingMode: BindingMode.OneWay);

    public string CapturingPlaceholderText
    {
        get => GetValue(CapturingPlaceholderTextProperty);
        set => SetValue(CapturingPlaceholderTextProperty, value);
    }
    
    public static readonly DirectProperty<HotkeyBox, string> PlaceholderTextProperty =
        AvaloniaProperty.RegisterDirect<HotkeyBox, string>(nameof(PlaceholderText), c => c.PlaceholderText);

    public string PlaceholderText
    {
        get;
        private set => SetAndRaise(PlaceholderTextProperty, ref field, value);
    } = string.Empty;
    #endregion PlaceholderText
    
    #region IsCapturing
    public static readonly DirectProperty<HotkeyBox, bool> IsCapturingProperty
        = AvaloniaProperty.RegisterDirect<HotkeyBox, bool>(nameof(IsCapturing), c => c.IsCapturing);

    public bool IsCapturing
    {
        get;
        private set
        {
            if (!SetAndRaise(IsCapturingProperty, ref field, value))
                return;
            PseudoClasses.Set(":capturing", value);
            UpdatePlaceholderText();
        }
    }
    #endregion

    private TextBox? _textBox;
        
    public HotkeyBox()
    {
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
        UpdateDisplayText();
        UpdatePlaceholderText();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_textBox is not null)
        {
            _textBox.GotFocus -= OnGotFocus;
            _textBox.LostFocus -= OnLostFocus;
        }
        
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");

        if (_textBox is not null)
        {
            _textBox.GotFocus += OnGotFocus;
            _textBox.LostFocus += OnLostFocus;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == EmptyPlaceholderTextProperty ||
            change.Property == CapturingPlaceholderTextProperty)
        {
            UpdatePlaceholderText();
        }
    }

    [RelayCommand]
    private void Clear()
    {
        KeyGesture = new KeyGesture(Key.None);
    }

    private void UpdateDisplayText()
    {
        DisplayText = KeyGesture.Key == Key.None
            ? string.Empty
            : KeyGesture.ToString();
    }

    private void UpdatePlaceholderText()
    {
        PlaceholderText = IsCapturing
            ? CapturingPlaceholderText
            : EmptyPlaceholderText;
    }

    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Tab or Key.Enter or Key.Escape)
            return;
        e.Handled = true;
        switch (e.Key)
        {
            case Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt 
                or Key.LWin or Key.RWin or Key.LeftShift or Key.RightShift:
                return;
            case Key.Delete:
            case Key.Back:
                Clear();
                return;
            default:
                KeyGesture = new KeyGesture(e.Key, e.KeyModifiers);
                break;
        }
    }

    private void OnGotFocus(object? sender, FocusChangedEventArgs e)
    {
        IsCapturing = true;
    }

    private void OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        IsCapturing = false;
    }
}
