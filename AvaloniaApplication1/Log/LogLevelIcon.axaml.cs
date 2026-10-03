using System;
using Avalonia;
using Avalonia.Controls;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Log;

public partial class LogLevelIcon : UserControl
{
    public static readonly StyledProperty<LogLevel> LevelProperty =
        AvaloniaProperty.Register<LogLevelIcon, LogLevel>(nameof(Level));
    
    public static readonly DirectProperty<LogLevelIcon, string?> IconPathProperty =
        AvaloniaProperty.RegisterDirect<LogLevelIcon, string?>(nameof(IconPath), o => o.IconPath);
    
    public static readonly DirectProperty<LogLevelIcon, SemanticType> SemanticTypeProperty =
        AvaloniaProperty.RegisterDirect<LogLevelIcon, SemanticType>(nameof(SemanticType), o => o.SemanticType);

    public LogLevel Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public string? IconPath
    {
        get;
        private set => SetAndRaise(IconPathProperty, ref field, value);
    }

    public SemanticType SemanticType
    {
        get;
        private set => SetAndRaise(SemanticTypeProperty, ref field, value);
    }

    public LogLevelIcon()
    {
        InitializeComponent();
        
        this.GetObservable(LevelProperty)
            .Subscribe(level =>
            {
                IconPath = LogLevelIconPathResolver.Resolve(level);
                SemanticType = LogLevelSemanticMapper.Map(level);
            });
    }
}