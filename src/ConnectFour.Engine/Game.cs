namespace ConnectFour.Engine;

public sealed class Game
{
    private Board _board;
    private Player _currentPlayer;
    private GameStatus _status;
    private Player? _winner;
    private IReadOnlyList<Position> _winningLine;

    public Game(int rows = 6, int columns = 7)
    {
        _board = new Board(rows, columns);
        _currentPlayer = Player.Blue;
        _status = GameStatus.InProgress;
        _winner = null;
        _winningLine = Array.Empty<Position>();
    }

    public Board Board => _board;
    public Player CurrentPlayer => _currentPlayer;
    public GameStatus Status => _status;
    public Player? Winner => _winner;
    public IReadOnlyList<Position> WinningLine => _winningLine;

#pragma warning disable CS0067 // Event not used in skeleton; wired up in full implementation
    public event Action<MoveResult>? MovePlayed;
#pragma warning restore CS0067

    public bool TryPlay(int column, out MoveResult result) =>
        throw new NotImplementedException();

    public void Reset() => throw new NotImplementedException();
}
