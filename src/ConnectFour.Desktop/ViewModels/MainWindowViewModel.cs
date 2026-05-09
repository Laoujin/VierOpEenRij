using ConnectFour.Desktop.Models;
using ConnectFour.Desktop.Services;

namespace ConnectFour.Desktop.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public GameViewModel Game { get; }

    public MainWindowViewModel(ISoundService sound)
    {
        Game = new GameViewModel(sound: sound);
    }

    public MainWindowViewModel() : this(new NoOpSoundService()) { }

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
