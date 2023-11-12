using System.Net;
using System.Text.Json;
using MessagePack;
using MessagePack.Resolvers;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class InputOutputFormatWafTests : GlobalWafTest
{
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
        var mp = await Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        Assert.Equal(MessagePackConstants.ContentType, MessagePackConstants.ContentType);
        
        var response = MessagePackSerializer.Deserialize<MessagePackInputResponse>(await mp.Content.ReadAsStreamAsync(), ser);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), response.PackedAt);
        Assert.Equal("IO Test", response.Test);
    }
    
    [Fact]
    public async Task TestOutputAsMessagePack()
    {
        var mp = await Client.GetByteArrayAsync("mp-output");
        var response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(mp, new MessagePackSerializerOptions(ContractlessStandardResolver.Instance));
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestOutputWithMessagePack(bool withAcceptsHeader)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output");
        if (withAcceptsHeader)
        {
            req.Headers.Add("Accept", MessagePackConstants.ContentType);
        }
        var mp = await Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        MessagePackOutputResponse response;
        if (withAcceptsHeader)
        {
            Assert.Equal(MessagePackConstants.ContentType, mp.Content.Headers.ContentType?.ToString());
            response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new MessagePackSerializerOptions(ContractlessStandardResolver.Instance));
        }
        else
        {
            Assert.Equal("application/json", mp.Content.Headers.ContentType?.ToString());
            response = JsonSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});            
        }
        
        Assert.Equal("Hello World!", response.Test);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestSendOutputWithMessagePack(bool withAcceptsHeader)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "vary-output-send");
        if (withAcceptsHeader)
        {
            req.Headers.Add("Accept", MessagePackConstants.ContentType);
        }
        var mp = await Client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, mp.StatusCode);
        MessagePackOutputResponse response;
        if (withAcceptsHeader)
        {
            Assert.Equal(MessagePackConstants.ContentType, mp.Content.Headers.ContentType?.ToString());
            response = MessagePackSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new MessagePackSerializerOptions(ContractlessStandardResolver.Instance));
        }
        else
        {
            Assert.Equal("application/json", mp.Content.Headers.ContentType?.ToString());
            response = JsonSerializer.Deserialize<MessagePackOutputResponse>(await mp.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});            
        }
        Assert.Equal("Hello World!", response.Test);
    }
    
}