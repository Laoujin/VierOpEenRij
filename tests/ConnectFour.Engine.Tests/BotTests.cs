using FluentAssertions;
using Xunit;

namespace ConnectFour.Engine.Tests;

public class BotTests
{
    [Fact]
    public void Bot_plays_winning_move_when_available()
    {
        var game = BoardBuilder.FromArt(@"
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . . . . . .
            B B B . . . .
        ", nextToMove: Player.Blue);

        var bot = new MinimaxBot(depth: 5, rng: new Random(42));

        bot.ChooseColumn(game).Should().Be(3);
    }

    [Fact]
    public void Bot_blocks_opponent_immediate_win()
    {
        var game = BoardBuilder.FromArt(@"
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . . . . . .
            R R R . . . .
        ", nextToMove: Player.Blue);

        var bot = new MinimaxBot(depth: 5, rng: new Random(42));

        bot.ChooseColumn(game).Should().Be(3);
    }

    [Fact]
    public void Bot_prefers_center_column_on_empty_board()
    {
        var game = new Game();
        var bot = new MinimaxBot(depth: 5, rng: new Random(0));

        bot.ChooseColumn(game).Should().Be(3);
    }

    [Fact]
    public void Bot_never_returns_invalid_column()
    {
        var game = BoardBuilder.FromArt(@"
            R B R . R B R
            B R B R B R B
            R B R B R B R
            B R B R B R B
            R B R B R B R
            B R B R B R B
        ", nextToMove: Player.Blue);

        var bot = new MinimaxBot(depth: 3, rng: new Random(0));

        var pick = bot.ChooseColumn(game);

        pick.Should().Be(3);
        game.Board.IsColumnFull(pick).Should().BeFalse();
    }
}
