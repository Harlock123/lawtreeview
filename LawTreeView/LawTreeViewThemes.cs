using Avalonia.Media;

namespace LawTreeView.Controls;

public record LawTreeViewThemeData(
    IBrush Background,
    IBrush ItemBackground,
    IBrush Foreground,
    IBrush SelectedItemBackground,
    IBrush SelectedItemForeground,
    IBrush ItemHoverBackground);

public static class LawTreeViewThemes
{
    private static IBrush B(string hex) => new SolidColorBrush(Color.Parse(hex));
    private static IBrush Overlay(byte a, byte r, byte g, byte b) =>
        new SolidColorBrush(Color.FromArgb(a, r, g, b));

    public static LawTreeViewThemeData Light { get; } = new(
        Background:             B("#EEF2F7"),
        ItemBackground:         Brushes.White,
        Foreground:             B("#222222"),
        SelectedItemBackground: B("#0078D4"),
        SelectedItemForeground: Brushes.White,
        ItemHoverBackground:    Overlay(0x40, 0, 0, 0));

    public static LawTreeViewThemeData Dark { get; } = new(
        Background:             B("#1E1E1E"),
        ItemBackground:         B("#252526"),
        Foreground:             Brushes.White,
        SelectedItemBackground: B("#0078D4"),
        SelectedItemForeground: Brushes.White,
        ItemHoverBackground:    Overlay(0x50, 0xFF, 0xFF, 0xFF));

    public static LawTreeViewThemeData Sepia { get; } = new(
        Background:             B("#F5EBD8"),
        ItemBackground:         B("#FAF3E0"),
        Foreground:             B("#5C4A35"),
        SelectedItemBackground: B("#A8845C"),
        SelectedItemForeground: Brushes.White,
        ItemHoverBackground:    Overlay(0x40, 0x5C, 0x4A, 0x35));

    // Solarized — https://ethanschoonover.com/solarized/
    public static LawTreeViewThemeData SolarizedLight { get; } = new(
        Background:             B("#FDF6E3"), // base3
        ItemBackground:         B("#EEE8D5"), // base2
        Foreground:             B("#586E75"), // base01
        SelectedItemBackground: B("#268BD2"), // blue
        SelectedItemForeground: B("#FDF6E3"),
        ItemHoverBackground:    Overlay(0x30, 0x58, 0x6E, 0x75));

    public static LawTreeViewThemeData SolarizedDark { get; } = new(
        Background:             B("#002B36"), // base03
        ItemBackground:         B("#073642"), // base02
        Foreground:             B("#93A1A1"), // base1
        SelectedItemBackground: B("#268BD2"), // blue
        SelectedItemForeground: B("#FDF6E3"),
        ItemHoverBackground:    Overlay(0x40, 0x93, 0xA1, 0xA1));

    // Nord — https://www.nordtheme.com/
    public static LawTreeViewThemeData Nord { get; } = new(
        Background:             B("#2E3440"), // nord0
        ItemBackground:         B("#3B4252"), // nord1
        Foreground:             B("#D8DEE9"), // nord4
        SelectedItemBackground: B("#5E81AC"), // nord10
        SelectedItemForeground: B("#ECEFF4"), // nord6
        ItemHoverBackground:    Overlay(0x40, 0xD8, 0xDE, 0xE9));

    // Dracula — https://draculatheme.com/
    public static LawTreeViewThemeData Dracula { get; } = new(
        Background:             B("#282A36"),
        ItemBackground:         B("#44475A"),
        Foreground:             B("#F8F8F2"),
        SelectedItemBackground: B("#BD93F9"),
        SelectedItemForeground: B("#282A36"),
        ItemHoverBackground:    Overlay(0x40, 0xF8, 0xF8, 0xF2));

    public static LawTreeViewThemeData Monokai { get; } = new(
        Background:             B("#272822"),
        ItemBackground:         B("#3E3D32"),
        Foreground:             B("#F8F8F2"),
        SelectedItemBackground: B("#75715E"),
        SelectedItemForeground: B("#F8F8F2"),
        ItemHoverBackground:    Overlay(0x40, 0xF8, 0xF8, 0xF2));

    public static LawTreeViewThemeData HighContrast { get; } = new(
        Background:             Brushes.Black,
        ItemBackground:         Brushes.Black,
        Foreground:             Brushes.White,
        SelectedItemBackground: Brushes.White,
        SelectedItemForeground: Brushes.Black,
        ItemHoverBackground:    Overlay(0x80, 0xFF, 0xFF, 0xFF));

    public static void Apply(this LawTreeView tree, LawTreeViewThemeData theme)
    {
        tree.Background             = theme.Background;
        tree.ItemBackground         = theme.ItemBackground;
        tree.Foreground             = theme.Foreground;
        tree.SelectedItemBackground = theme.SelectedItemBackground;
        tree.SelectedItemForeground = theme.SelectedItemForeground;
        tree.ItemHoverBackground    = theme.ItemHoverBackground;
    }
}
