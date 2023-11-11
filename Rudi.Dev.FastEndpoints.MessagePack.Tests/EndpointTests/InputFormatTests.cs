using System.Net;
using System.Text.Json;
using FastEndpoints.Testing;
using MessagePack;
using MessagePack.Resolvers;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;
using Xunit.Abstractions;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class InputFormatTests : TestClass<GlobalFixture>
{
    public InputFormatTests(GlobalFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

    [Fact]
    public async Task TestInputOutput()
    {
        var request = new MessagePackInputRequest
        {
            Test = "IO Test"
        };
        var ser = new MessagePackSerializerOptions(ContractlessStandardResolver.Instance);
        var requestBytes = MessagePackSerializer.Serialize(request, ser);
        var req = new HttpRequestMessage(HttpMethod.Post, "mp-input");
        req.Content = new ByteArrayContent(requestBytes);
        req.Content.Headers.Add("Content-Type", MessagePackConstants.ContentType);
        var mp = await Fixture.Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, MessagePackConstants.ContentType);
        
        var response = MessagePackSerializer.Deserialize<MessagePackInputResponse>(await mp.Content.ReadAsStreamAsync(), ser);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), response.PackedAt);
        Assert.Equal("IO Test", response.Test);
    }
}