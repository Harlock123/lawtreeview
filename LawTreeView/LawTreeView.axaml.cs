using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace LawTreeView.Controls;

public partial class LawTreeView : UserControl
{
    public static readonly StyledProperty<IEnumerable<LawTreeViewItem>?> ItemsSourceProperty =
        AvaloniaProperty.Register<LawTreeView, IEnumerable<LawTreeViewItem>?>(nameof(ItemsSource));

    public static readonly StyledProperty<IBrush?> ItemBackgroundProperty =
        AvaloniaProperty.Register<LawTreeView, IBrush?>(
            nameof(ItemBackground),
            Brushes.Transparent);

    public static readonly StyledProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.Register<LawTreeView, IBrush?>(
            nameof(ItemHoverBackground),
            new SolidColorBrush(Color.FromArgb(0x40, 0, 0, 0)));

    public static readonly StyledProperty<IBrush?> SelectedItemBackgroundProperty =
        AvaloniaProperty.Register<LawTreeView, IBrush?>(
            nameof(SelectedItemBackground),
            new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4)));

    public static readonly StyledProperty<IBrush?> SelectedItemForegroundProperty =
        AvaloniaProperty.Register<LawTreeView, IBrush?>(
            nameof(SelectedItemForeground),
            Brushes.White);

    public static readonly StyledProperty<LawTreeViewItem?> SelectedItemProperty =
        AvaloniaProperty.Register<LawTreeView, LawTreeViewItem?>(
            nameof(SelectedItem),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> AllowDragDropProperty =
        AvaloniaProperty.Register<LawTreeView, bool>(nameof(AllowDragDrop), false);

    public IEnumerable<LawTreeViewItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IBrush? ItemBackground
    {
        get => GetValue(ItemBackgroundProperty);
        set => SetValue(ItemBackgroundProperty, value);
    }

    public IBrush? ItemHoverBackground
    {
        get => GetValue(ItemHoverBackgroundProperty);
        set => SetValue(ItemHoverBackgroundProperty, value);
    }

    public IBrush? SelectedItemBackground
    {
        get => GetValue(SelectedItemBackgroundProperty);
        set => SetValue(SelectedItemBackgroundProperty, value);
    }

    public IBrush? SelectedItemForeground
    {
        get => GetValue(SelectedItemForegroundProperty);
        set => SetValue(SelectedItemForegroundProperty, value);
    }

    public LawTreeViewItem? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public bool AllowDragDrop
    {
        get => GetValue(AllowDragDropProperty);
        set => SetValue(AllowDragDropProperty, value);
    }

    public event EventHandler<LawTreeViewSelectionChangedEventArgs>? SelectionChanged;
    public event EventHandler<LawTreeViewItemDroppedEventArgs>? ItemDropped;

    private ItemsControl? _root;

    public LawTreeView()
    {
        InitializeComponent();
        _root = this.FindControl<ItemsControl>("PART_Root");
        Focusable = true;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty && _root != null)
        {
            _root.ItemsSource = ItemsSource;
        }
        else if (change.Property == SelectedItemProperty)
        {
            var oldItem = change.OldValue as LawTreeViewItem;
            var newItem = change.NewValue as LawTreeViewItem;
            if (oldItem != null) oldItem.IsSelected = false;
            if (newItem != null) newItem.IsSelected = true;
            SelectionChanged?.Invoke(this, new LawTreeViewSelectionChangedEventArgs(oldItem, newItem));
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled) return;

        var (visible, parents) = BuildVisibleList();
        if (visible.Count == 0) return;

        var current = SelectedItem;
        var index = current != null ? visible.IndexOf(current) : -1;

        switch (e.Key)
        {
            case Key.Up:
                if (index > 0) SelectedItem = visible[index - 1];
                else if (index < 0) SelectedItem = visible[0];
                e.Handled = true;
                break;

            case Key.Down:
                if (index >= 0 && index < visible.Count - 1) SelectedItem = visible[index + 1];
                else if (index < 0) SelectedItem = visible[0];
                e.Handled = true;
                break;

            case Key.Left:
                if (current != null)
                {
                    if (current.HasChildren && current.IsExpanded)
                        current.IsExpanded = false;
                    else if (parents.TryGetValue(current, out var parent))
                        SelectedItem = parent;
                    e.Handled = true;
                }
                break;

            case Key.Right:
                if (current != null && current.HasChildren)
                {
                    if (!current.IsExpanded) current.IsExpanded = true;
                    else if (current.Children.Count > 0) SelectedItem = current.Children[0];
                    e.Handled = true;
                }
                break;

            case Key.Home:
                SelectedItem = visible[0];
                e.Handled = true;
                break;

            case Key.End:
                SelectedItem = visible[visible.Count - 1];
                e.Handled = true;
                break;

            case Key.Enter:
            case Key.Space:
                if (current != null && current.HasChildren)
                    current.IsExpanded = !current.IsExpanded;
                e.Handled = true;
                break;
        }
    }

    internal void PerformDrop(LawTreeViewItem dragged, LawTreeViewItem target, bool dropAbove)
    {
        if (ReferenceEquals(dragged, target)) return;
        if (IsAncestorOf(dragged, target)) return;

        if (!TryFindParent(dragged, out var sourceParent, out int sourceIndex)) return;
        if (!TryFindParent(target, out var targetParent, out int targetIndex)) return;

        var sourceCol = GetCollection(sourceParent);
        var targetCol = GetCollection(targetParent);
        if (sourceCol == null || targetCol == null) return;

        int newIndex = dropAbove ? targetIndex : targetIndex + 1;

        sourceCol.RemoveAt(sourceIndex);

        if (ReferenceEquals(sourceCol, targetCol) && sourceIndex < newIndex)
            newIndex--;

        targetCol.Insert(newIndex, dragged);

        ItemDropped?.Invoke(this, new LawTreeViewItemDroppedEventArgs(
            dragged, sourceParent, sourceIndex, targetParent, newIndex));
    }

    private bool TryFindParent(LawTreeViewItem target, out LawTreeViewItem? parent, out int index)
    {
        parent = null;
        index = -1;
        if (ItemsSource == null) return false;

        int i = 0;
        foreach (var root in ItemsSource)
        {
            if (ReferenceEquals(root, target)) { parent = null; index = i; return true; }
            if (TryFindIn(root, target, out parent, out index)) return true;
            i++;
        }
        return false;
    }

    private static bool TryFindIn(LawTreeViewItem node, LawTreeViewItem target,
                                   out LawTreeViewItem? parent, out int index)
    {
        for (int i = 0; i < node.Children.Count; i++)
        {
            if (ReferenceEquals(node.Children[i], target)) { parent = node; index = i; return true; }
            if (TryFindIn(node.Children[i], target, out parent, out index)) return true;
        }
        parent = null;
        index = -1;
        return false;
    }

    private IList<LawTreeViewItem>? GetCollection(LawTreeViewItem? parent)
    {
        if (parent != null) return parent.Children;
        return ItemsSource as IList<LawTreeViewItem>;
    }

    private static bool IsAncestorOf(LawTreeViewItem candidate, LawTreeViewItem target)
    {
        foreach (var child in candidate.Children)
        {
            if (ReferenceEquals(child, target)) return true;
            if (IsAncestorOf(child, target)) return true;
        }
        return false;
    }

    private (List<LawTreeViewItem> items, Dictionary<LawTreeViewItem, LawTreeViewItem> parents) BuildVisibleList()
    {
        var items = new List<LawTreeViewItem>();
        var parents = new Dictionary<LawTreeViewItem, LawTreeViewItem>();
        if (ItemsSource == null) return (items, parents);

        void Walk(LawTreeViewItem item, LawTreeViewItem? parent)
        {
            items.Add(item);
            if (parent != null) parents[item] = parent;
            if (item.IsExpanded && item.HasChildren)
                foreach (var child in item.Children)
                    Walk(child, item);
        }

        foreach (var root in ItemsSource) Walk(root, null);
        return (items, parents);
    }
}

public class LawTreeViewSelectionChangedEventArgs : EventArgs
{
    public LawTreeViewItem? OldItem { get; }
    public LawTreeViewItem? NewItem { get; }

    public LawTreeViewSelectionChangedEventArgs(LawTreeViewItem? oldItem, LawTreeViewItem? newItem)
    {
        OldItem = oldItem;
        NewItem = newItem;
    }
}

public class LawTreeViewItemDroppedEventArgs : EventArgs
{
    public LawTreeViewItem Item { get; }
    public LawTreeViewItem? SourceParent { get; }
    public int SourceIndex { get; }
    public LawTreeViewItem? TargetParent { get; }
    public int TargetIndex { get; }

    public LawTreeViewItemDroppedEventArgs(
        LawTreeViewItem item,
        LawTreeViewItem? sourceParent, int sourceIndex,
        LawTreeViewItem? targetParent, int targetIndex)
    {
        Item = item;
        SourceParent = sourceParent;
        SourceIndex = sourceIndex;
        TargetParent = targetParent;
        TargetIndex = targetIndex;
    }
}
