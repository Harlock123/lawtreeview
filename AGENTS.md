# AGENTS.md

Notes for anyone, human or AI agent, changing this code. `README.md` documents the control's public interface,
interactions and usage; this file covers how the code is put together and how to change it safely.

## What this is

LawTreeView, a hierarchical tree-view control for Avalonia 11 (C#, .NET 9), meant to be filled from code. It has
drag-drop reordering, icons, single selection, keyboard navigation, hover and selection styling, a context menu
and ready-made colour themes. The library is set up to pack as a NuGet package, and a demo app exercises it.

## Commands

```bash
dotnet build LawTreeView.sln                         # net9.0, Avalonia 11.2.1; there is no global.json
dotnet run --project LawTreeView.Demo                # the demo window with a toolbar and sample data
dotnet pack LawTreeView/LawTreeView.csproj -c Release    # writes the .nupkg and .snupkg under LawTreeView/bin/Release
```

There are no tests yet. Check changes by hand in the demo.

## Layout

- `LawTreeView/`: the class library (namespace `LawTreeView.Controls`).
  - `LawTreeView.axaml` and `.axaml.cs`: the `UserControl`. Styled properties (`ItemsSource`, `SelectedItem`,
    `AllowDragDrop`, the item brushes), the `SelectionChanged` and `ItemDropped` events, and keyboard navigation
    in `OnKeyDown`.
  - `LawTreeViewItemControl.axaml` and `.axaml.cs`: the template for one row, used recursively for children.
    Handles tapping, double-tapping, the expander, the context menu, and drag and drop (5-pixel threshold).
  - `LawTreeViewItem.cs`: the data model, a plain `INotifyPropertyChanged` class with `Children`, `IsExpanded`,
    `IsSelected`, `Icon`, and computed helpers such as `HasChildren` and `ExpanderSymbol`.
  - `LawTreeViewThemes.cs`: `LawTreeViewThemeData` records (Light, Dark, Sepia, Nord, Dracula, Monokai,
    HighContrast) and the `Apply` extension method.
  - `README.md`: the package readme, shipped inside the `.nupkg`; it is separate from the root `README.md`.
- `LawTreeView.Demo/`: the sample app (`MainWindow` wires a toolbar to the tree).

## Rules that keep it working

- **The public interface is the package's contract.** The root `README.md` documents every styled property,
  event and model member. Change it on purpose, and update both READMEs when you do.
- **The model has no Avalonia dependency** apart from what an icon value may hold. Keep `LawTreeViewItem`
  a plain class so callers can build trees anywhere.
- **Compiled bindings are on** (`AvaloniaUseCompiledBindingsByDefault`). Templates need `x:DataType`, and a wrong
  binding path fails the build rather than failing quietly.
- **Collections must notify.** Children are `ObservableCollection<LawTreeViewItem>`, and drag-drop moves items
  between those collections, so a plain `List<>` as `ItemsSource` will not refresh the UI.
- **Drag-drop is opt-in** through `AllowDragDrop`; drops onto the item itself or into its own descendants are
  rejected.

## Making a change

- Build with no new warnings, then try the change in the demo: selection, keyboard, context menu, drag-drop
  and each theme.
- Raise `<Version>` in `LawTreeView/LawTreeView.csproj` before packing a new release. Its `Authors` and `Company`
  are still placeholders (`YOUR_NAME`, `YOUR_COMPANY`), and `PackageProjectUrl` and `RepositoryUrl` are empty;
  fill them in before publishing to NuGet.

There is no CI workflow in this repository.
