# LawTreeView

An Avalonia 11 tree-view control for .NET 9 with built-in drag-drop reordering, icons, selection, keyboard navigation, hover/selection styling, and a context menu.

## Install

```
dotnet add package LawTreeView
```

## Quick start

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:law="clr-namespace:LawTreeView.Controls;assembly=LawTreeView">
    <law:LawTreeView Name="Tree" AllowDragDrop="True" />
</Window>
```

```csharp
using LawTreeView.Controls;

Tree.ItemsSource = new ObservableCollection<LawTreeViewItem>
{
    new()
    {
        DisplayMember = "Cases",
        Children =
        {
            new LawTreeViewItem { DisplayMember = "Smith v. Jones" },
            new LawTreeViewItem { DisplayMember = "Doe v. Acme Corp" },
        }
    }
};

Tree.SelectionChanged += (_, e) => Console.WriteLine($"Selected: {e.NewItem?.DisplayMember}");
Tree.ItemDropped += (_, e) =>
    Console.WriteLine($"Moved {e.Item.DisplayMember}: " +
                      $"{e.SourceParent?.DisplayMember ?? "(root)"}[{e.SourceIndex}] -> " +
                      $"{e.TargetParent?.DisplayMember ?? "(root)"}[{e.TargetIndex}]");
```

## Properties

| Property | Type | Notes |
|---|---|---|
| `ItemsSource` | `IEnumerable<LawTreeViewItem>?` | Use `ObservableCollection` for live top-level mutations |
| `SelectedItem` | `LawTreeViewItem?` | Two-way bindable |
| `AllowDragDrop` | `bool` | Default `false` |
| `ItemBackground` | `IBrush?` | Per-row background |
| `ItemHoverBackground` | `IBrush?` | Background under the pointer |
| `SelectedItemBackground` | `IBrush?` | Selected row background |
| `SelectedItemForeground` | `IBrush?` | Selected row foreground (also recolors `Geometry` icons) |

Inherited Avalonia properties — `FontFamily`, `FontSize`, `FontWeight`, `FontStyle`, `Foreground`, `Background` — cascade to all items.

## Events

- `SelectionChanged(LawTreeViewSelectionChangedEventArgs)` — `OldItem`, `NewItem`
- `ItemDropped(LawTreeViewItemDroppedEventArgs)` — `Item`, `SourceParent`, `SourceIndex`, `TargetParent`, `TargetIndex`

## LawTreeViewItem

- `DisplayMember` (string)
- `Children` (ObservableCollection&lt;LawTreeViewItem&gt;)
- `IsExpanded`, `IsSelected`
- `Icon` (object — accepts `Geometry` for vectors or `Bitmap` for raster)
- `ExpandAll()`, `CollapseAll()`

## Keyboard

| Key | Action |
|---|---|
| ↑ ↓ | Move selection |
| ← | Collapse, or jump to parent |
| → | Expand, or jump to first child |
| Home / End | First / last visible item |
| Enter / Space | Toggle expand/collapse |

## License

MIT
