using System.Reflection;

namespace ConnectFour.Engine.Tests;

internal static class BoardBuilder
{
    /// <summary>
    /// Build a Game whose board matches the given ASCII art. Top row first.
    /// 'B' = Blue, 'R' = Red, '.' = Empty. Spaces and pipes are ignored.
    /// </summary>
    public static Game FromArt(string art, Player nextToMove = Player.Blue)
    {
        var lines = art.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Select(l => l.Replace(" ", "").Replace("|", "").TrimEnd('\r'))
                       .Where(l => l.Length > 0)
                       .ToArray();
        int rows = lines.Length;
        int cols = lines[0].Length;
        var game = new Game(rows, cols);
        var boardField = typeof(Game).GetField("_board", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var statusField = typeof(Game).GetField("_status", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var currentField = typeof(Game).GetField("_currentPlayer", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var cellsField = typeof(Board).GetField("_cells", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var board = new Board(rows, cols);
        var cells = (CellState[,])cellsField.GetValue(board)!;
        for (int r = 0; r < rows; r++)
        {
            var line = lines[r];
            if (line.Length != cols)
                throw new ArgumentException($"Row {r} has {line.Length} cells, expected {cols}.");
            for (int c = 0; c < cols; c++)
            {
                cells[r, c] = line[c] switch
                {
                    'B' => CellState.Blue,
                    'R' => CellState.Red,
                    '.' => CellState.Empty,
                    _ => throw new ArgumentException($"Bad char '{line[c]}' at row {r} col {c}.")
                };
            }
        }
        boardField.SetValue(game, board);
        currentField.SetValue(game, nextToMove);
        statusField.SetValue(game, GameStatus.InProgress);
        return game;
    }
}
