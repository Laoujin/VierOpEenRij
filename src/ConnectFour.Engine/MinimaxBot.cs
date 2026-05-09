namespace ConnectFour.Engine;

public sealed class MinimaxBot : IBot
{
    private readonly int _depth;
    private readonly Random _rng;

    private const int WinScore = 1_000_000;

    public MinimaxBot(int depth = 5, Random? rng = null)
    {
        if (depth < 1) throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be at least 1.");
        _depth = depth;
        _rng = rng ?? Random.Shared;
    }

    public int ChooseColumn(Game game)
    {
        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is not in progress.");

        var perspective = game.CurrentPlayer;
        int bestScore = int.MinValue;
        var bestColumns = new List<int>();
        int alpha = -WinScore - 1;

        for (int col = 0; col < game.Board.Columns; col++)
        {
            if (game.Board.IsColumnFull(col)) continue;

            var simBoard = game.Board.PlaceDisc(col, perspective, out var landing);
            var winLine = WinDetection.FindWinningLine(simBoard, landing, perspective);

            int score;
            if (winLine.Count > 0)
            {
                score = WinScore;
            }
            else if (simBoard.IsFull)
            {
                score = 0;
            }
            else
            {
                // perspective just moved; opponent moves next — return value from perspective's view
                score = Minimax(simBoard, perspective.Opponent(), perspective, _depth - 1,
                    alpha, WinScore + 1);
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestColumns.Clear();
                bestColumns.Add(col);
            }
            else if (score == bestScore)
            {
                bestColumns.Add(col);
            }

            if (bestScore > alpha) alpha = bestScore;
        }

        if (bestColumns.Count == 0)
            throw new InvalidOperationException("No valid moves available.");

        return bestColumns.Count == 1
            ? bestColumns[0]
            : bestColumns[_rng.Next(bestColumns.Count)];
    }

    // Returns score from `perspective`'s point of view.
    // `toMove` is whose turn it is in this sub-tree.
    private static int Minimax(Board board, Player toMove, Player perspective, int depth, int alpha, int beta)
    {
        if (depth == 0)
            return Evaluate(board, perspective);

        bool maximizing = toMove == perspective;
        int best = maximizing ? int.MinValue : int.MaxValue;
        bool anyMove = false;

        for (int col = 0; col < board.Columns; col++)
        {
            if (board.IsColumnFull(col)) continue;
            anyMove = true;

            var newBoard = board.PlaceDisc(col, toMove, out var landing);
            var winLine = WinDetection.FindWinningLine(newBoard, landing, toMove);

            int value;
            if (winLine.Count > 0)
            {
                // toMove just won; score relative to perspective
                value = (toMove == perspective)
                    ? WinScore - (10 - depth)
                    : -(WinScore - (10 - depth));
            }
            else if (newBoard.IsFull)
            {
                value = 0;
            }
            else
            {
                value = Minimax(newBoard, toMove.Opponent(), perspective, depth - 1, alpha, beta);
            }

            if (maximizing)
            {
                if (value > best) best = value;
                if (best > alpha) alpha = best;
            }
            else
            {
                if (value < best) best = value;
                if (best < beta) beta = best;
            }

            if (alpha >= beta) break;
        }

        return anyMove ? best : 0;
    }

    private static int Evaluate(Board board, Player perspective)
    {
        var us = perspective.ToCellState();
        var them = perspective.Opponent().ToCellState();

        int score = ScoreSide(board, us) - ScoreSide(board, them);

        int centerCol = board.Columns / 2;
        for (int r = 0; r < board.Rows; r++)
        {
            if (board[r, centerCol] == us) score += 3;
            else if (board[r, centerCol] == them) score -= 3;
        }
        return score;
    }

    private static int ScoreSide(Board board, CellState side)
    {
        int score = 0;
        (int dr, int dc)[] dirs = { (0, 1), (1, 0), (1, 1), (1, -1) };

        for (int r = 0; r < board.Rows; r++)
        {
            for (int c = 0; c < board.Columns; c++)
            {
                foreach (var (dr, dc) in dirs)
                {
                    int er = r + 3 * dr, ec = c + 3 * dc;
                    if (!board.IsInBounds(er, ec)) continue;

                    int sideCount = 0, emptyCount = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        var cell = board[r + k * dr, c + k * dc];
                        if (cell == side) sideCount++;
                        else if (cell == CellState.Empty) emptyCount++;
                        else { sideCount = -1; break; }
                    }
                    if (sideCount < 0) continue;
                    if (sideCount == 3 && emptyCount == 1) score += 50;
                    else if (sideCount == 2 && emptyCount == 2) score += 10;
                    else if (sideCount == 1 && emptyCount == 3) score += 1;
                }
            }
        }
        return score;
    }
}
