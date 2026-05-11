namespace ConnectFour.Engine;

public static class PlayerExtensions
{
    public static CellState ToCellState(this Player player) => (CellState)player;

    public static Player Opponent(this Player player) =>
        player == Player.Blue ? Player.Red : Player.Blue;
}
