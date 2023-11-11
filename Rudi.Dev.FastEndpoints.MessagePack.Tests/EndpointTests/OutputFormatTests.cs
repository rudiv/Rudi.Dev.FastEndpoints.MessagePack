using System.Net;
using System.Text.Json;
using FastEndpoints.Testing;
using MessagePack;
using MessagePack.Resolvers;
using NuGet.Frameworks;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;
using Xunit.Abstractions;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class OutputFormatTests : TestClass<GlobalFixture>
{
    public OutputFormatTests(GlobalFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

    [Fact]
    public async Task TestOutputAsMessagePack()
    {
        var mp = await Fixture.Client.GetByteArrayAsync("mp-output");
        var response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(mp, new MessagePackSerializerOptions(ContractlessStandardResolver.Instance));
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Fact]
    public async Task TestOutputWithMessagePack_NoAccept()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output");
        var mp = await Fixture.Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal("application/json", mp.Content.Headers.ContentType?.ToString());
        
        var response = JsonSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Fact]
    public async Task TestOutputWithMessagePack_WithAccept()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output");
        req.Headers.Add("Accept", MessagePackConstants.ContentType);
        var mp = await Fixture.Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, mp.Content.Headers.ContentType?.ToString());
        
        var response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new MessagePackSerializerOptions(ContractlessStandardResolver.Instance));
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Fact]
    public async Task TestSendOutputWithMessagePack_NoAccept()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output-send");
        var mp = await Fixture.Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal("application/json", mp.Content.Headers.ContentType?.ToString());
        
        var response = JsonSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Fact]
    public async Task TestSendOutputWithMessagePack_WithAccept()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output-send");
        req.Headers.Add("Accept", MessagePackConstants.ContentType);
        var mp = await Fixture.Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, mp.Content.Headers.ContentType?.ToString());
        
        var response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new MessagePackSerializerOptions(ContractlessStandardResolver.Instance));
        Assert.Equal("Hello World!", response.Test);
    }
}