using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ConnectFour.Desktop.ViewModels;

namespace ConnectFour.Desktop.Views;

public partial class BoardView : UserControl
{
    public BoardView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnColumnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int col && DataContext is GameViewModel vm)
            vm.HoverColumn(col);
    }

    private void OnColumnPointerExited(object? sender, PointerEventArgs e)
    {
        if (DataContext is GameViewModel vm)
            vm.HoverColumn(null);
    }
}
