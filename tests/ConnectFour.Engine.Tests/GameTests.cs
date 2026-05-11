using System.Reflection;
using FluentAssertions;
using Xunit;

namespace ConnectFour.Engine.Tests;

public class GameTests
{
    [Fact]
    public void New_game_has_empty_board_with_blue_to_move()
    {
        var game = new Game();

        game.Board.Rows.Should().Be(6);
        game.Board.Columns.Should().Be(7);
        game.CurrentPlayer.Should().Be(Player.Blue);
        game.Status.Should().Be(GameStatus.InProgress);
        game.Winner.Should().BeNull();
        game.WinningLine.Should().BeEmpty();

        for (int r = 0; r < game.Board.Rows; r++)
            for (int c = 0; c < game.Board.Columns; c++)
                game.Board[r, c].Should().Be(CellState.Empty);
    }

    [Fact]
    public void TryPlay_lands_disc_in_bottom_row_of_empty_column()
    {
        var game = new Game();

        var ok = game.TryPlay(3, out var result);

        ok.Should().BeTrue();
        result.Landing.Should().Be(new Position(5, 3));
        result.Status.Should().Be(GameStatus.InProgress);
        game.Board[5, 3].Should().Be(CellState.Blue);
        game.CurrentPlayer.Should().Be(Player.Red);
    }

    [Fact]
    public void Stacked_discs_land_on_top_of_each_other()
    {
        var game = new Game();

        game.PlayMoves(3, 3, 3);   // blue, red, blue all in column 3

        game.Board[5, 3].Should().Be(CellState.Blue);
        game.Board[4, 3].Should().Be(CellState.Red);
        game.Board[3, 3].Should().Be(CellState.Blue);
        game.Board[2, 3].Should().Be(CellState.Empty);
        game.CurrentPlayer.Should().Be(Player.Red);
    }

    [Fact]
    public void TryPlay_returns_false_when_column_is_full()
    {
        var game = new Game();
        game.PlayMoves(0, 0, 0, 0, 0, 0);   // 6 discs filling column 0

        var ok = game.TryPlay(0, out var result);

        ok.Should().BeFalse();
        result.Should().BeNull();
        game.Status.Should().Be(GameStatus.InProgress);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    [InlineData(99)]
    public void TryPlay_returns_false_for_out_of_range_column(int column)
    {
        var game = new Game();

        var ok = game.TryPlay(column, out var result);

        ok.Should().BeFalse();
        result.Should().BeNull();
        game.CurrentPlayer.Should().Be(Player.Blue);
    }

    [Fact]
    public void Horizontal_four_in_a_row_wins()
    {
        var game = new Game();
        // Blue plays cols 0,1,2,3 ; Red fills row above-but-not-decisive
        game.PlayMoves(0, 0, 1, 1, 2, 2, 3);

        game.Status.Should().Be(GameStatus.Won);
        game.Winner.Should().Be(Player.Blue);
        game.WinningLine.Should().BeEquivalentTo(new[]
        {
            new Position(5, 0),
            new Position(5, 1),
            new Position(5, 2),
            new Position(5, 3),
        });
    }

    [Fact]
    public void Vertical_four_in_a_row_wins()
    {
        var game = new Game();
        // Blue plays col 3 four times, Red plays col 0 thrice in between
        game.PlayMoves(3, 0, 3, 0, 3, 0, 3);

        game.Status.Should().Be(GameStatus.Won);
        game.Winner.Should().Be(Player.Blue);
        game.WinningLine.Should().BeEquivalentTo(new[]
        {
            new Position(2, 3),
            new Position(3, 3),
            new Position(4, 3),
            new Position(5, 3),
        });
    }

    [Fact]
    public void DiagonalDown_four_in_a_row_wins()
    {
        // Col 3 has 3 Red fillers so Blue's disc lands at row 2; diagonal ↘ completes (2,0)-(3,1)-(4,2)-(5,3).
        var game = BoardBuilder.FromArt(@"
            . . . . . . .
            . . . . . . .
            B . . . . . .
            R B . . . . .
            R R B . . . .
            R R R . . . .
        ", nextToMove: Player.Blue);

        var ok = game.TryPlay(3, out var result);

        ok.Should().BeTrue();
        game.Status.Should().Be(GameStatus.Won);
        game.Winner.Should().Be(Player.Blue);
        game.WinningLine.Should().BeEquivalentTo(new[]
        {
            new Position(2, 0),
            new Position(3, 1),
            new Position(4, 2),
            new Position(5, 3),
        });
    }

    [Fact]
    public void DiagonalUp_four_in_a_row_wins()
    {
        // Col 3 has 3 Red fillers so Blue's disc lands at row 2; diagonal ↗ completes (5,0)-(4,1)-(3,2)-(2,3).
        var game = BoardBuilder.FromArt(@"
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . B R . . .
            . B R R . . .
            B R R R . . .
        ", nextToMove: Player.Blue);

        var ok = game.TryPlay(3, out var result);

        ok.Should().BeTrue();
        game.Status.Should().Be(GameStatus.Won);
        game.Winner.Should().Be(Player.Blue);
        game.WinningLine.Should().BeEquivalentTo(new[]
        {
            new Position(5, 0),
            new Position(4, 1),
            new Position(3, 2),
            new Position(2, 3),
        });
    }

    [Fact]
    public void Filled_board_with_no_four_is_a_draw()
    {
        // Board pattern: rows alternate RBBRRBB / BRRBBRR — no 4-in-a-row in any direction.
        var game = BoardBuilder.FromArt(@"
            R B B R R B B
            B R R B B R R
            R B B R R B B
            B R R B B R R
            R B B R R B B
            B R R B B R R
        ", nextToMove: Player.Blue);

        game.Board.IsFull.Should().BeTrue();

        typeof(Game).GetMethod("RecomputeTerminalStatus", BindingFlags.NonPublic | BindingFlags.Instance)
            !.Invoke(game, null);

        game.Status.Should().Be(GameStatus.Draw);
        game.Winner.Should().BeNull();
        game.WinningLine.Should().BeEmpty();
    }
}
