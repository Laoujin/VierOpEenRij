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
