namespace ConnectFour.Desktop.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    public GameViewModel Game { get; } = new();
}
