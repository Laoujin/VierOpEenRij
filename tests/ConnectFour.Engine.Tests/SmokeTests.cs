using FluentAssertions;
using Xunit;

namespace ConnectFour.Engine.Tests;

public class SmokeTests
{
    [Fact]
    public void Engine_assembly_is_referenced()
    {
        Placeholder.Marker.Should().Be("engine-scaffold");
    }
}
