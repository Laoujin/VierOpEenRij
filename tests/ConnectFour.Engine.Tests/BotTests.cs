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
}
