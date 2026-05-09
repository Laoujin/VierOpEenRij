using System.Reflection;

namespace ConnectFour.Engine.Tests;

internal static class TestHelpers
{
    public static void PlayMoves(this Game game, params int[] columns)
    {
        foreach (var col in columns)
        {
            if (!game.TryPlay(col, out _))
                throw new InvalidOperationException($"Move on column {col} was rejected.");
        }
    }
}

internal static class BoardBuilder
{
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
