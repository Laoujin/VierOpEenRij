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
}
