namespace ConnectFour.Engine;

public sealed record MoveResult(
    Board Board,
    Position Landing,
    GameStatus Status,
    Player? Winner,
    IReadOnlyList<Position> WinningLine);
