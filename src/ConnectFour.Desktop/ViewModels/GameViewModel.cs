using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConnectFour.Desktop.Models;
using ConnectFour.Engine;

namespace ConnectFour.Desktop.ViewModels;

public sealed partial class GameViewModel : ViewModelBase
{
    private readonly Game _game;
    private readonly IBot _bot;

    public ObservableCollection<CellViewModel> Cells { get; }
    public IReadOnlyList<int> ColumnHandles { get; }
    public int Rows => _game.Board.Rows;
    public int Columns => _game.Board.Columns;

    [ObservableProperty]
    private GameMode _mode = GameMode.HotSeat;

    [ObservableProperty]
    private string _statusText = "Blue to move";

    [ObservableProperty]
    private bool _isThinking;

    public GameViewModel(IBot? bot = null)
    {
        _game = new Game();
        _bot = bot ?? new MinimaxBot();
        Cells = new ObservableCollection<CellViewModel>();
        for (int r = 0; r < _game.Board.Rows; r++)
            for (int c = 0; c < _game.Board.Columns; c++)
                Cells.Add(new CellViewModel(r, c));
        ColumnHandles = Enumerable.Range(0, _game.Board.Columns).ToList();
        _game.MovePlayed += OnMovePlayed;
    }

    [RelayCommand(CanExecute = nameof(CanPlay))]
    private async Task PlayColumn(int column)
    {
        if (!_game.TryPlay(column, out _))
            return;

        if (_game.Status == GameStatus.InProgress && IsBotTurn)
            await PlayBotMoveAsync();
    }

    private bool CanPlay(int column) =>
        !IsThinking && _game.Status == GameStatus.InProgress && !IsBotTurn;

    private bool IsBotTurn =>
        Mode == GameMode.BotVsBot ||
        (Mode == GameMode.VsBot && _game.CurrentPlayer == Player.Red);

    private async Task PlayBotMoveAsync()
    {
        IsThinking = true;
        StatusText = "Thinking…";
        try
        {
            while (_game.Status == GameStatus.InProgress && IsBotTurn)
            {
                int col = await Task.Run(() => _bot.ChooseColumn(_game));
                _game.TryPlay(col, out _);
            }
        }
        finally
        {
            IsThinking = false;
        }
    }

    [RelayCommand]
    private void NewGame()
    {
        _game.Reset();
        foreach (var cell in Cells)
        {
            cell.State = CellState.Empty;
            cell.IsWinningCell = false;
            cell.IsLandingPreview = false;
        }
        StatusText = "Blue to move";
        PlayColumnCommand.NotifyCanExecuteChanged();

        if (IsBotTurn)
            _ = PlayBotMoveAsync();
    }

    private void OnMovePlayed(MoveResult result)
    {
        var cell = Cells.First(c => c.Row == result.Landing.Row && c.Column == result.Landing.Column);
        cell.State = result.Board[result.Landing.Row, result.Landing.Column];

        StatusText = result.Status switch
        {
            GameStatus.Won  => $"{result.Winner} wins!",
            GameStatus.Draw => "Draw.",
            _               => $"{_game.CurrentPlayer} to move"
        };

        if (result.Status == GameStatus.Won)
        {
            foreach (var pos in result.WinningLine)
            {
                var winCell = Cells.First(c => c.Row == pos.Row && c.Column == pos.Column);
                winCell.IsWinningCell = true;
            }
        }

        PlayColumnCommand.NotifyCanExecuteChanged();
    }

    partial void OnModeChanged(GameMode value) => NewGame();
}
