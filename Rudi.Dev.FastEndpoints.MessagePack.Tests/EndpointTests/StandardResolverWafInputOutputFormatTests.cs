using System.Net;
using MessagePack;
using MessagePack.Resolvers;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class StandardResolverWafInputOutputFormatTests : StandardResolverWafTest
{
    [Fact]
    public async Task TestInputOutput()
    {
        var request = new MessagePackStandardInputRequest
        {
            Test = "IO Test"
        };
        var ser = new MessagePackSerializerOptions(StandardResolver.Instance);
        var requestBytes = MessagePackSerializer.Serialize(request, ser);
        var req = new HttpRequestMessage(HttpMethod.Post, "mp-input-std");
        req.Content = new ByteArrayContent(requestBytes);
        req.Content.Headers.Add("Content-Type", MessagePackConstants.ContentType);
        var mp = await Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, MessagePackConstants.ContentType);
        
        var response = MessagePackSerializer.Deserialize<MessagePackStandardInputResponse>(await mp.Content.ReadAsStreamAsync(), ser);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), response.PackedAt);
        Assert.Equal("IO Test", response.Test);
    }
    
    [Fact]
    public async Task TestOutputAsMessagePack()
    {
        var mp = await Client.GetByteArrayAsync("mp-output-std");
        var response = MessagePackSerializer.Deserialize<MessagePackStandardOutputResponse>(mp, new MessagePackSerializerOptions(StandardResolver.Instance));
        Assert.Equal("Hello World!", response.Test);
    }
}