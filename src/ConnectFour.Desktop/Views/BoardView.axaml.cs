using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ConnectFour.Desktop.ViewModels;

namespace ConnectFour.Desktop.Views;

public partial class BoardView : UserControl
{
    public BoardView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    // Route button click to the PlayColumnCommand on the GameViewModel.
    // The cast syntax for cross-DataTemplate command bindings is unreliable
    // in the runtime XAML parser, so we dispatch via code-behind instead.
    private void OnColumnButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int column } && DataContext is GameViewModel vm)
            vm.PlayColumnCommand.Execute(column);
    }

    private void OnColumnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int col && DataContext is GameViewModel vm)
            vm.HoverColumn(col);
    }

    private void OnColumnPointerExited(object? sender, PointerEventArgs e)
    {
        if (DataContext is GameViewModel vm)
            vm.HoverColumn(null);
    }
}
