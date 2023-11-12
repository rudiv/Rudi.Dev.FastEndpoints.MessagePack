using System.Net;
using System.Text.Json;
using MessagePack;
using MessagePack.Resolvers;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.EndpointTests;

public class NoInputBinderWafTests : NoInputBinderWafTest
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
        Assert.Equal(HttpStatusCode.BadRequest, mp.StatusCode);
        Assert.Equal("application/problem+json", mp.Content.Headers.ContentType?.ToString());
        
        var resp = JsonSerializer.Deserialize<TempErrors>(await mp.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Assert.Equal("test", resp.Errors.First().Key);
        Assert.Equal("'Test' must not be empty.", resp.Errors.First().Value.First());
    }

    public class TempErrors
    {
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}