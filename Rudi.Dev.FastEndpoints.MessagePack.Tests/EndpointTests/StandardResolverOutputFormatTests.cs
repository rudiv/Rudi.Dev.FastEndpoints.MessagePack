using System.Net;
using System.Text.Json;
using FastEndpoints.Testing;
using MessagePack;
using MessagePack.Resolvers;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;
using Xunit.Abstractions;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class StandardResolverOutputFormatTests : TestClass<StandardResolverFixture>
{
    public StandardResolverOutputFormatTests(StandardResolverFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

    [Fact]
    public async Task TestOutputAsMessagePack()
    {
        var mp = await Fixture.Client.GetByteArrayAsync("mp-output-std");
        var response = MessagePackSerializer.Deserialize<MessagePackStandardOutputResponse>(mp, new MessagePackSerializerOptions(StandardResolver.Instance));
        Assert.Equal("Hello World!", response.Test);
    }
}