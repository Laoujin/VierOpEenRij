using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ConnectFour.Desktop.Views;

public partial class BoardView : UserControl
{
    public BoardView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
