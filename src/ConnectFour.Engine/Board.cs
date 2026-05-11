namespace ConnectFour.Engine;

public sealed class Board
{
    private readonly CellState[,] _cells;

    public int Rows { get; }
    public int Columns { get; }

    public Board(int rows = 6, int columns = 7)
    {
        if (rows < 4) throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be at least 4.");
        if (columns < 4) throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be at least 4.");
        Rows = rows;
        Columns = columns;
        _cells = new CellState[rows, columns];
    }

    private Board(CellState[,] cells, int rows, int columns)
    {
        _cells = cells;
        Rows = rows;
        Columns = columns;
    }

    public CellState this[int row, int col] => _cells[row, col];

    public bool IsInBounds(int row, int col) =>
        row >= 0 && row < Rows && col >= 0 && col < Columns;

    public bool IsColumnFull(int column)
    {
        if (column < 0 || column >= Columns) throw new ArgumentOutOfRangeException(nameof(column));
        return _cells[0, column] != CellState.Empty;
    }

    public bool IsFull
    {
        get
        {
            for (int c = 0; c < Columns; c++)
                if (_cells[0, c] == CellState.Empty) return false;
            return true;
        }
    }

    public Board PlaceDisc(int column, Player player, out Position landing)
    {
        if (column < 0 || column >= Columns)
            throw new ArgumentOutOfRangeException(nameof(column));
        if (IsColumnFull(column))
            throw new InvalidOperationException($"Column {column} is full.");

        int landRow = -1;
        for (int r = Rows - 1; r >= 0; r--)
        {
            if (_cells[r, column] == CellState.Empty)
            {
                landRow = r;
                break;
            }
        }

        var newCells = (CellState[,])_cells.Clone();
        newCells[landRow, column] = player.ToCellState();
        landing = new Position(landRow, column);
        return new Board(newCells, Rows, Columns);
    }
}
