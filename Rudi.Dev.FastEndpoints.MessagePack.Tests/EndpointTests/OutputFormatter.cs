using FastEndpoints;
using FastEndpoints.Testing;
using MessagePack;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;
using Xunit.Abstractions;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class OutputFormatter : TestClass<GlobalFixture>
{
    public OutputFormatter(GlobalFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

    [Fact]
    public async Task TestOutputIsMessagePack()
    {
        var mp = await Fixture.Client.GetByteArrayAsync("output");
        var response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(mp);
        Assert.Equal("Hello World!", response.Test);
    }
}