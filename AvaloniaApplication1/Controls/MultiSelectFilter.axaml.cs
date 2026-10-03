using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AvaloniaApplication1.Controls;

public class MultiSelectFilter : TemplatedControl
{
    public static readonly StyledProperty<object?> SummaryContentProperty =
        AvaloniaProperty.Register<MultiSelectFilter, object?>(nameof(SummaryContent));

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        ItemsControl.ItemTemplateProperty.AddOwner<MultiSelectFilter>();

    /*public static readonly StyledProperty<IDataTemplate?> ItemContentTemplateProperty =
        AvaloniaProperty.Register<MultiSelectFilter, IDataTemplate?>(nameof(ItemContentTemplate));*/
    
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        ItemsControl.ItemsSourceProperty.AddOwner<MultiSelectFilter>();

    public static readonly StyledProperty<int> MaxFlyoutHeightProperty =
        AvaloniaProperty.Register<MultiSelectFilter, int>(nameof(MaxFlyoutHeight));
    

    /*private static readonly IDataTemplate DefaultItemContentTemplate =
        new FuncDataTemplate<ILabeledSelectable>(
            _ => true, 
            s => new TextBlock { Text = s.Label }
            );*/
    
    public object? SummaryContent
    {
        get => GetValue(SummaryContentProperty);
        set => SetValue(SummaryContentProperty, value);
    }
    
    /*public IDataTemplate? ItemContentTemplate
    {
        get => GetValue(ItemContentTemplateProperty);
        set => SetValue(ItemContentTemplateProperty, value);
    }*/
    
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
    
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int MaxFlyoutHeight
    {
        get => GetValue(MaxFlyoutHeightProperty);
        set => SetValue(MaxFlyoutHeightProperty, value);
    }
    
    /*public static IDataTemplate? GetItemContentTemplatePassthrough(ItemsControl element) =>
        element.GetValue(ItemContentTemplatePassthroughProperty);
    
    public static void SetItemContentTemplatePassthrough(ItemsControl element, IDataTemplate? value) =>
        element.SetValue(ItemContentTemplatePassthroughProperty, value);*/
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e.NameScope.Find<ItemsControl>("PART_ItemsControl") is not { } itemsControl) 
            return;
        
        itemsControl.DataTemplates.Clear();
        itemsControl.DataTemplates.AddRange(DataTemplates);
    }
}