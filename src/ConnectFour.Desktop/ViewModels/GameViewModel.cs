using CommunityToolkit.Mvvm.ComponentModel;

namespace ConnectFour.Desktop.ViewModels;

// Minimal placeholder — fully implemented in the next commit
public sealed partial class GameViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _statusText = "Connect Four";
}
