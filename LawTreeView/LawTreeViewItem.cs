using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LawTreeView.Controls;

public class LawTreeViewItem : INotifyPropertyChanged
{
    private string _displayMember = string.Empty;
    private bool _isExpanded;
    private bool _isSelected;
    private object? _icon;
    private ObservableCollection<LawTreeViewItem> _children;

    public LawTreeViewItem()
    {
        _children = new ObservableCollection<LawTreeViewItem>();
        _children.CollectionChanged += OnChildrenChanged;
    }

    public string DisplayMember
    {
        get => _displayMember;
        set
        {
            if (_displayMember == value) return;
            _displayMember = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<LawTreeViewItem> Children
    {
        get => _children;
        set
        {
            if (ReferenceEquals(_children, value)) return;
            if (_children != null)
                _children.CollectionChanged -= OnChildrenChanged;
            _children = value ?? new ObservableCollection<LawTreeViewItem>();
            _children.CollectionChanged += OnChildrenChanged;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasChildren));
            OnPropertyChanged(nameof(ExpanderSymbol));
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ExpanderSymbol));
            OnPropertyChanged(nameof(CanExpand));
            OnPropertyChanged(nameof(CanCollapse));
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public object? Icon
    {
        get => _icon;
        set
        {
            if (Equals(_icon, value)) return;
            _icon = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasIcon));
        }
    }

    public bool HasIcon => _icon != null;

    public bool HasChildren => _children != null && _children.Count > 0;

    public bool CanExpand => HasChildren && !IsExpanded;

    public bool CanCollapse => HasChildren && IsExpanded;

    public string ExpanderSymbol => !HasChildren ? string.Empty : (IsExpanded ? "−" : "+");

    public void ExpandAll()
    {
        if (!HasChildren) return;
        IsExpanded = true;
        foreach (var c in _children) c.ExpandAll();
    }

    public void CollapseAll()
    {
        IsExpanded = false;
        foreach (var c in _children) c.CollapseAll();
    }

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasChildren));
        OnPropertyChanged(nameof(ExpanderSymbol));
        OnPropertyChanged(nameof(CanExpand));
        OnPropertyChanged(nameof(CanCollapse));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
