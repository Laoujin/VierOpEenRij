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

    public event Action<MoveResult>? MovePlayed;

    public bool TryPlay(int column, out MoveResult result)
    {
        result = default!;
        if (_status != GameStatus.InProgress) return false;
        if (column < 0 || column >= _board.Columns) return false;
        if (_board.IsColumnFull(column)) return false;

        var movingPlayer = _currentPlayer;
        var newBoard = _board.PlaceDisc(column, movingPlayer, out var landing);
        var winningLine = WinDetection.FindWinningLine(newBoard, landing, movingPlayer);

        GameStatus newStatus;
        Player? newWinner;
        if (winningLine.Count > 0)
        {
            newStatus = GameStatus.Won;
            newWinner = movingPlayer;
        }
        else if (newBoard.IsFull)
        {
            newStatus = GameStatus.Draw;
            newWinner = null;
        }
        else
        {
            newStatus = GameStatus.InProgress;
            newWinner = null;
        }

        _board = newBoard;
        _status = newStatus;
        _winner = newWinner;
        _winningLine = winningLine;
        if (newStatus == GameStatus.InProgress)
            _currentPlayer = movingPlayer.Opponent();

        result = new MoveResult(newBoard, landing, newStatus, newWinner, winningLine);
        MovePlayed?.Invoke(result);
        return true;
    }

    public void Reset()
    {
        _board = new Board(_board.Rows, _board.Columns);
        _currentPlayer = Player.Blue;
        _status = GameStatus.InProgress;
        _winner = null;
        _winningLine = Array.Empty<Position>();
    }

    internal void RecomputeTerminalStatus()
    {
        if (_status != GameStatus.InProgress) return;
        if (_board.IsFull)
        {
            _status = GameStatus.Draw;
            _winner = null;
            _winningLine = Array.Empty<Position>();
        }
    }
}
