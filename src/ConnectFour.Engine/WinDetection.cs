namespace ConnectFour.Engine;

internal static class WinDetection
{
    private static readonly (int dRow, int dCol)[] Directions =
    {
        (0, 1),   // horizontal →
        (1, 0),   // vertical   ↓
        (1, 1),   // diagonal   ↘
        (1, -1)   // diagonal   ↙
    };

    public static IReadOnlyList<Position> FindWinningLine(Board board, Position landing, Player player)
    {
        var target = player.ToCellState();
        foreach (var (dRow, dCol) in Directions)
        {
            int r = landing.Row;
            int c = landing.Column;

            while (board.IsInBounds(r - dRow, c - dCol) && board[r - dRow, c - dCol] == target)
            {
                r -= dRow;
                c -= dCol;
            }

            var line = new List<Position>();
            while (board.IsInBounds(r, c) && board[r, c] == target)
            {
                line.Add(new Position(r, c));
                r += dRow;
                c += dCol;
            }

            if (line.Count >= 4)
                return line.GetRange(0, 4);
        }
        return Array.Empty<Position>();
    }
}
