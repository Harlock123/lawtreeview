using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using LawTreeView.Controls;

namespace LawTreeView.Demo;

public partial class MainWindow : Window
{
    private static readonly Geometry FolderIcon = Geometry.Parse(
        "M10,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V8C22,6.89 21.1,6 20,6H12L10,4Z");

    private static readonly Geometry DocIcon = Geometry.Parse(
        "M14,2H6A2,2 0 0,0 4,4V20A2,2 0 0,0 6,22H18A2,2 0 0,0 20,20V8L14,2M18,20H6V4H13V9H18V20Z");

    private static readonly Geometry PersonIcon = Geometry.Parse(
        "M12,4A4,4 0 0,1 16,8A4,4 0 0,1 12,12A4,4 0 0,1 8,8A4,4 0 0,1 12,4M12,14C16.42,14 20,15.79 20,18V20H4V18C4,15.79 7.58,14 12,14Z");

    private static readonly Geometry CalendarIcon = Geometry.Parse(
        "M19,3H18V1H16V3H8V1H6V3H5C3.89,3 3,3.9 3,5V21A2,2 0 0,0 5,23H19A2,2 0 0,0 21,21V5C21,3.89 20.1,3 19,3M19,21H5V8H19V21Z");

    public MainWindow()
    {
        InitializeComponent();

        Tree.ItemsSource = BuildSampleData();
        Tree.AllowDragDrop = true;
        ApplyTheme(0);

        DragDropToggle.IsChecked = Tree.AllowDragDrop;
        DragDropToggle.Click += (_, _) => Tree.AllowDragDrop = DragDropToggle.IsChecked == true;

        SizeSlider.Value = Tree.FontSize;
        SizeSlider.ValueChanged += (_, e) => Tree.FontSize = e.NewValue;

        BoldBtn.Click += (_, _) =>
            Tree.FontWeight = BoldBtn.IsChecked == true ? FontWeight.Bold : FontWeight.Normal;
        ItalicBtn.Click += (_, _) =>
            Tree.FontStyle = ItalicBtn.IsChecked == true ? FontStyle.Italic : FontStyle.Normal;

        ThemeBox.SelectionChanged += (_, _) => ApplyTheme(ThemeBox.SelectedIndex);

        Tree.SelectionChanged += (_, e) =>
            Title = e.NewItem != null
                ? $"LawTreeView.Demo — {e.NewItem.DisplayMember}"
                : "LawTreeView.Demo";
        Tree.ItemDropped += (_, e) =>
            Title = $"Moved {e.Item.DisplayMember}: " +
                    $"{e.SourceParent?.DisplayMember ?? "(root)"}[{e.SourceIndex}] → " +
                    $"{e.TargetParent?.DisplayMember ?? "(root)"}[{e.TargetIndex}]";
    }

    private void ApplyTheme(int index)
    {
        var theme = index switch
        {
            0 => LawTreeViewThemes.Light,
            1 => LawTreeViewThemes.Dark,
            2 => LawTreeViewThemes.Sepia,
            3 => LawTreeViewThemes.SolarizedLight,
            4 => LawTreeViewThemes.SolarizedDark,
            5 => LawTreeViewThemes.Nord,
            6 => LawTreeViewThemes.Dracula,
            7 => LawTreeViewThemes.Monokai,
            8 => LawTreeViewThemes.HighContrast,
            _ => LawTreeViewThemes.Light,
        };
        Tree.Apply(theme);
    }

    private static ObservableCollection<LawTreeViewItem> BuildSampleData()
    {
        return new ObservableCollection<LawTreeViewItem>
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
                            new LawTreeViewItem { DisplayMember = "Pleadings", Icon = DocIcon },
                            new LawTreeViewItem
                            {
                                DisplayMember = "Discovery", Icon = FolderIcon,
                                Children =
                                {
                                    new LawTreeViewItem { DisplayMember = "Interrogatories", Icon = DocIcon },
                                    new LawTreeViewItem { DisplayMember = "Depositions", Icon = DocIcon },
                                }
                            },
                            new LawTreeViewItem { DisplayMember = "Motions", Icon = DocIcon },
                        }
                    },
                    new LawTreeViewItem
                    {
                        DisplayMember = "Doe v. Acme Corp", Icon = FolderIcon,
                        Children =
                        {
                            new LawTreeViewItem { DisplayMember = "Complaint", Icon = DocIcon },
                            new LawTreeViewItem { DisplayMember = "Answer", Icon = DocIcon },
                        }
                    }
                }
            },
            new()
            {
                DisplayMember = "Clients", Icon = FolderIcon,
                Children =
                {
                    new LawTreeViewItem { DisplayMember = "Smith, John", Icon = PersonIcon },
                    new LawTreeViewItem { DisplayMember = "Doe, Jane", Icon = PersonIcon },
                }
            },
            new() { DisplayMember = "Calendar", Icon = CalendarIcon },
        };
    }
}
