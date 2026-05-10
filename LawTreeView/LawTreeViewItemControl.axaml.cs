using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace LawTreeView.Controls;

public partial class LawTreeViewItemControl : UserControl
{
    private const string DragDataFormat = "LawTreeViewItem";
    private const double DragThresholdSquared = 25; // 5px

    private Point _dragStart;
    private bool _isPotentialDrag;

    public LawTreeViewItemControl()
    {
        InitializeComponent();
        var row = this.FindControl<StackPanel>("PART_Row");
        if (row != null)
        {
            row.PointerPressed += OnRowPointerPressed;
            row.PointerMoved += OnRowPointerMoved;
            row.PointerReleased += OnRowPointerReleased;
            row.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            row.AddHandler(DragDrop.DropEvent, OnDrop);
        }
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private LawTreeView? Tree => this.FindAncestorOfType<LawTreeView>();

    private void OnExpanderTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is LawTreeViewItem item)
            item.IsExpanded = !item.IsExpanded;
        e.Handled = true;
    }

    private void OnRowTapped(object? sender, TappedEventArgs e)
    {
        if (IsTapInsideExpander(e)) return;
        if (DataContext is LawTreeViewItem item)
        {
            var tree = Tree;
            if (tree != null)
            {
                tree.SelectedItem = item;
                tree.Focus();
            }
        }
    }

    private void OnRowDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (IsTapInsideExpander(e)) return;
        if (DataContext is LawTreeViewItem item && item.HasChildren)
            item.IsExpanded = !item.IsExpanded;
    }

    private void OnRowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Tree?.AllowDragDrop != true) return;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        _dragStart = e.GetPosition(this);
        _isPotentialDrag = true;
    }

    private async void OnRowPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPotentialDrag) return;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isPotentialDrag = false;
            return;
        }

        var pos = e.GetPosition(this);
        var dx = pos.X - _dragStart.X;
        var dy = pos.Y - _dragStart.Y;
        if (dx * dx + dy * dy < DragThresholdSquared) return;

        _isPotentialDrag = false;

        if (DataContext is LawTreeViewItem item)
        {
            var data = new DataObject();
            data.Set(DragDataFormat, item);
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
        }
    }

    private void OnRowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isPotentialDrag = false;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
        if (Tree?.AllowDragDrop != true) return;
        if (!e.Data.Contains(DragDataFormat)) return;
        if (e.Data.Get(DragDataFormat) is not LawTreeViewItem dragged) return;
        if (DataContext is not LawTreeViewItem target) return;
        if (ReferenceEquals(dragged, target)) return;
        if (IsAncestorOf(dragged, target)) return;

        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (Tree is not { AllowDragDrop: true } tree) return;
        if (!e.Data.Contains(DragDataFormat)) return;
        if (e.Data.Get(DragDataFormat) is not LawTreeViewItem dragged) return;
        if (DataContext is not LawTreeViewItem target) return;
        if (sender is not Visual rowVisual) return;

        var pos = e.GetPosition(rowVisual);
        bool dropAbove = pos.Y < rowVisual.Bounds.Height / 2;

        tree.PerformDrop(dragged, target, dropAbove);
        e.Handled = true;
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

    private static bool IsTapInsideExpander(TappedEventArgs e)
    {
        if (e.Source is not Visual v) return false;
        foreach (var ancestor in v.GetSelfAndVisualAncestors())
        {
            if (ancestor is Border b && b.Classes.Contains("expander"))
                return true;
        }
        return false;
    }

    private void OnMenuExpand(object? sender, RoutedEventArgs e)
    {
        if (DataContext is LawTreeViewItem item) item.IsExpanded = true;
    }

    private void OnMenuCollapse(object? sender, RoutedEventArgs e)
    {
        if (DataContext is LawTreeViewItem item) item.IsExpanded = false;
    }

    private void OnMenuExpandAll(object? sender, RoutedEventArgs e)
    {
        if (DataContext is LawTreeViewItem item) item.ExpandAll();
    }

    private void OnMenuCollapseAll(object? sender, RoutedEventArgs e)
    {
        if (DataContext is LawTreeViewItem item) item.CollapseAll();
    }
}
