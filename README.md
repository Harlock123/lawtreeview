# LawTreeView

An Avalonia 11 / .NET 9 hierarchical tree-view control. Designed to be programmatically populated, with built-in drag-drop reordering, icons, single selection, full keyboard navigation, hover/selection styling, and a context menu.

This repo contains both the publishable control library and a demo app that exercises every feature.

## Project structure

```
lawtreeview/
├── LawTreeView.sln
├── LawTreeView/                       # Publishable class library (NuGet)
│   ├── LawTreeView.csproj             # Package metadata
│   ├── README.md                      # Shipped inside the .nupkg
│   ├── LawTreeView.axaml + .cs        # The LawTreeView UserControl
│   ├── LawTreeViewItem.cs             # The hierarchical data model
│   └── LawTreeViewItemControl.axaml   # Per-row template (recursive)
│       + .cs
└── LawTreeView.Demo/                  # Reference / sandbox app
    ├── App.axaml + .cs
    ├── MainWindow.axaml + .cs         # Toolbar + Tree wired to sample data
    ├── Program.cs
    └── LawTreeView.Demo.csproj
```

## Build / run

```sh
# Build everything
dotnet build

# Run the demo (toolbar + sample legal hierarchy)
dotnet run --project LawTreeView.Demo

# Pack the NuGet
dotnet pack LawTreeView/LawTreeView.csproj -c Release
# → LawTreeView/bin/Release/LawTreeView.0.1.0.nupkg (+ .snupkg)
```

The demo's toolbar lets you toggle drag-drop, change font size, switch Bold/Italic, and pick between Light / Dark / Sepia themes — useful for exercising the styling properties.

## Public interface

### `LawTreeView` (control)

Namespace: `LawTreeView.Controls` · Assembly: `LawTreeView` · Base: `Avalonia.Controls.UserControl`

#### Styled properties

| Property                 | Type                           | Default              | Notes                                                                                  |
|--------------------------|--------------------------------|----------------------|----------------------------------------------------------------------------------------|
| `ItemsSource`            | `IEnumerable<LawTreeViewItem>?` | `null`               | Use `ObservableCollection<LawTreeViewItem>` if you want top-level mutations to refresh the UI. |
| `SelectedItem`           | `LawTreeViewItem?`              | `null`               | Two-way bindable. Setting it programmatically highlights the row and fires `SelectionChanged`. |
| `AllowDragDrop`          | `bool`                          | `false`              | Master switch for drag-drop reordering.                                                |
| `ItemBackground`         | `IBrush?`                       | `Transparent`        | Per-row background.                                                                    |
| `ItemHoverBackground`    | `IBrush?`                       | ~25% black overlay   | Background under the pointer.                                                          |
| `SelectedItemBackground` | `IBrush?`                       | `#0078D4` (Fluent blue) | Selected row background.                                                            |
| `SelectedItemForeground` | `IBrush?`                       | `White`              | Selected row text + recolors `Geometry`-based icons.                                   |

Inherited Avalonia properties also apply and cascade to all rows: `FontFamily`, `FontSize`, `FontWeight`, `FontStyle`, `Foreground`, `Background`.

#### Events

| Event              | Args                                       | Fields                                                                                 |
|--------------------|--------------------------------------------|----------------------------------------------------------------------------------------|
| `SelectionChanged` | `LawTreeViewSelectionChangedEventArgs`     | `OldItem`, `NewItem`                                                                   |
| `ItemDropped`      | `LawTreeViewItemDroppedEventArgs`          | `Item`, `SourceParent`, `SourceIndex`, `TargetParent`, `TargetIndex` — parents are `null` for top-level positions. |

### `LawTreeViewItem` (model)

A plain `INotifyPropertyChanged` object — instantiate directly and add to a collection. No Avalonia dependency for the model itself.

| Member             | Type / Signature                          | Notes                                                                                  |
|--------------------|-------------------------------------------|----------------------------------------------------------------------------------------|
| `DisplayMember`    | `string`                                  | Text rendered for the row.                                                             |
| `Children`         | `ObservableCollection<LawTreeViewItem>`   | Nested items. Mutations update the UI live.                                            |
| `IsExpanded`       | `bool`                                    | Toggles the children's visibility.                                                     |
| `IsSelected`       | `bool`                                    | Maintained automatically by the control when `SelectedItem` changes.                   |
| `Icon`             | `object?`                                 | Accepts `Avalonia.Media.Geometry` (vector, recolors with `Foreground`) or `Avalonia.Media.Imaging.Bitmap` (raster). Anything else is rendered via the default `ContentPresenter`. |
| `HasIcon`          | `bool` (computed)                         | Drives icon-slot visibility.                                                           |
| `HasChildren`      | `bool` (computed)                         | Drives expander visibility.                                                            |
| `CanExpand`        | `bool` (computed)                         | `HasChildren && !IsExpanded`. Used by the context menu's enable state.                 |
| `CanCollapse`      | `bool` (computed)                         | `HasChildren && IsExpanded`.                                                           |
| `ExpanderSymbol`   | `string` (computed)                       | `"+"`, `"−"`, or `""`.                                                                 |
| `ExpandAll()`      | `void`                                    | Recursively expand this node and every descendant with children.                       |
| `CollapseAll()`    | `void`                                    | Recursively collapse this node and every descendant.                                   |
| `PropertyChanged`  | `PropertyChangedEventHandler?`            | Standard `INotifyPropertyChanged`.                                                     |

### Built-in interactions

- **Tap a row** → selects it (`SelectedItem` updates, `SelectionChanged` fires).
- **Tap the `+/−`** → toggles expansion only (no selection change).
- **Double-tap a row** → toggles expansion. The expander itself is filtered out so you don't double-toggle.
- **Right-click a row** → context menu: `Expand` / `Collapse` / `Expand All` / `Collapse All`. Items grey out when not applicable.
- **Drag a row** (when `AllowDragDrop="True"`) → drop in the upper half of another row to insert *before* it; lower half to insert *after*. Cross-parent moves work naturally. Self-drops and ancestor→descendant drops are rejected.
- **Hover** → row background switches to `ItemHoverBackground`.

### Keyboard

| Key             | Action                                       |
|-----------------|----------------------------------------------|
| `↑` / `↓`       | Move selection through visible items         |
| `←`             | Collapse if expanded; otherwise jump to parent |
| `→`             | Expand if collapsed; otherwise jump to first child |
| `Home` / `End`  | First / last visible item                    |
| `Enter` / `Space` | Toggle expand/collapse on the current item |

Click any row first to focus the tree. The control sets `Focusable = true` and grabs focus on tap.

## XAML usage

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:law="clr-namespace:LawTreeView.Controls;assembly=LawTreeView"
        x:Class="MyApp.MainWindow"
        Title="My App" Width="600" Height="500">
    <Grid Margin="12">
        <law:LawTreeView Name="Tree"
                         AllowDragDrop="True"
                         FontFamily="Inter"
                         FontSize="14"
                         FontWeight="Normal"
                         Foreground="#222"
                         Background="#EEF2F7"
                         ItemBackground="White"
                         ItemHoverBackground="#D6E4F0"
                         SelectedItemBackground="#0078D4"
                         SelectedItemForeground="White" />
    </Grid>
</Window>
```

The control fills its container — drop it in a `Grid`, `DockPanel`, or `Border` and it resizes live with the parent.

## C# usage

```csharp
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using LawTreeView.Controls;

public partial class MainWindow : Window
{
    private static readonly Geometry FolderIcon = Geometry.Parse(
        "M10,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V8C22,6.89 21.1,6 20,6H12L10,4Z");
    private static readonly Geometry DocIcon = Geometry.Parse(
        "M14,2H6A2,2 0 0,0 4,4V20A2,2 0 0,0 6,22H18A2,2 0 0,0 20,20V8L14,2M18,20H6V4H13V9H18V20Z");

    public MainWindow()
    {
        InitializeComponent();

        // Build a hierarchy
        var data = new ObservableCollection<LawTreeViewItem>
        {
            new()
            {
                DisplayMember = "Cases", Icon = FolderIcon,
                Children =
                {
                    new LawTreeViewItem
                    {
                        DisplayMember = "Smith v. Jones", Icon = FolderIcon,
                        Children =
                        {
                            new LawTreeViewItem { DisplayMember = "Complaint",  Icon = DocIcon },
                            new LawTreeViewItem { DisplayMember = "Answer",     Icon = DocIcon },
                        }
                    },
                    new LawTreeViewItem { DisplayMember = "Doe v. Acme",        Icon = FolderIcon },
                }
            },
            new() { DisplayMember = "Calendar" },
        };

        Tree.ItemsSource    = data;
        Tree.AllowDragDrop  = true;

        // Selection
        Tree.SelectionChanged += (_, e) =>
        {
            var name = e.NewItem?.DisplayMember ?? "(none)";
            Title = $"Selected: {name}";
        };

        // Drag-drop
        Tree.ItemDropped += (_, e) =>
        {
            var from = e.SourceParent?.DisplayMember ?? "(root)";
            var to   = e.TargetParent?.DisplayMember ?? "(root)";
            Console.WriteLine(
                $"Moved '{e.Item.DisplayMember}': {from}[{e.SourceIndex}] -> {to}[{e.TargetIndex}]");
        };

        // Programmatic selection / expansion
        Tree.SelectedItem = data[0];
        data[0].ExpandAll();
    }
}
```

### Two-way binding `SelectedItem`

```csharp
public LawTreeViewItem? Current
{
    get => GetValue(CurrentProperty);
    set => SetValue(CurrentProperty, value);
}
```

```xml
<law:LawTreeView ItemsSource="{Binding Items}"
                 SelectedItem="{Binding Current, Mode=TwoWay}" />
```

## Notes / gotchas

- **Top-level reordering** requires `ItemsSource` to be a mutable, change-notifying list (`ObservableCollection<LawTreeViewItem>`). Plain `List<>` will mutate but the UI won't refresh.
- **Icons inherit color from the row's `Foreground`** when they're `Geometry` instances (rendered via `PathIcon`). When the row is selected, they switch to `SelectedItemForeground`. Bitmap icons keep their own colors.
- **Compiled bindings** are enabled (`AvaloniaUseCompiledBindingsByDefault=true`). All bindings inside the control's templates use `x:DataType` for compile-time checking.
- **Drag-drop** is opt-in via `AllowDragDrop`. The 5-pixel movement threshold prevents accidental drags from clicks.

## License

MIT
