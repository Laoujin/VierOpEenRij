using Avalonia.Controls;
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
}
