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

    [Fact]
    public void Bot_with_same_seed_returns_same_move()
    {
        var game = BoardBuilder.FromArt(@"
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . . . . . .
            . . . . B . .
            . . R B R . .
        ", nextToMove: Player.Blue);

        var bot1 = new MinimaxBot(depth: 5, rng: new Random(123));
        var bot2 = new MinimaxBot(depth: 5, rng: new Random(123));

        bot1.ChooseColumn(game).Should().Be(bot2.ChooseColumn(game));
    }

    [Fact]
    public void Bot_returns_within_one_second_at_depth_five()
    {
        var game = new Game();
        var bot = new MinimaxBot(depth: 5, rng: new Random(0));

        var sw = System.Diagnostics.Stopwatch.StartNew();
        bot.ChooseColumn(game);
        sw.Stop();

        sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Bot_tie_break_varies_across_unseeded_runs()
    {
        // Symmetric position with center full: cols 2 and 4 tie at equal heuristic distance.
        // Different RNG seeds must therefore produce different picks.
        var picks = new HashSet<int>();
        for (int i = 0; i < 50; i++)
        {
            var game = BoardBuilder.FromArt(@"
                . . . . . . .
                . . . . . . .
                . . . . . . .
                . . . . . . .
                . . . . . . .
                . . . B . . .
            ", nextToMove: Player.Blue);
            var bot = new MinimaxBot(depth: 1, rng: new Random(i));
            picks.Add(bot.ChooseColumn(game));
        }

        picks.Count.Should().BeGreaterThan(1, "tie-break should produce different columns under different seeds");
    }
}
