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
}
