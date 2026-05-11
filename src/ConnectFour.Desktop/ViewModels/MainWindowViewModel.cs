using ConnectFour.Desktop.Models;

namespace ConnectFour.Desktop.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    public GameViewModel Game { get; } = new();

    public IReadOnlyList<GameMode> Modes { get; } =
        [GameMode.HotSeat, GameMode.VsBot, GameMode.BotVsBot];

    public GameMode SelectedMode
    {
        get => Game.Mode;
        set
        {
            if (Game.Mode != value)
            {
                Game.Mode = value;
                OnPropertyChanged();
            }
        }
    }
}
