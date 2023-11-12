using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests;

public abstract class HttpContextTestBase
{
    
    private readonly HttpRequestFeature httpRequestFeature = new();
    private readonly FeatureCollection featureCollection = new();
    protected HttpContext HttpContext;

    public enum HeaderType
    {
        NotSet,
        Valid,
        Invalid
    }
    
    protected HttpRequest GetHttpRequest(HeaderType withAcceptHeader = HeaderType.NotSet, HeaderType withContentTypeHeader = HeaderType.NotSet)
    {
        if (HttpContext == default)
        {
            featureCollection.Set<IHttpRequestFeature>(httpRequestFeature);
            featureCollection.Set<IHttpResponseFeature>(new HttpResponseFeature());
            featureCollection.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(Stream.Null));
            HttpContext = new DefaultHttpContext(featureCollection);
            HttpContext.RequestServices = new ServiceCollection().AddSingleton<MessagePackOptions>().BuildServiceProvider();
        }
        httpRequestFeature.Headers.Clear();
        if (withAcceptHeader == HeaderType.Valid)
        {
            httpRequestFeature.Headers.Append("Accept", MessagePackConstants.ContentType);
        }
        else if (withAcceptHeader == HeaderType.Invalid)
        {
            httpRequestFeature.Headers.Append("Accept", "application/messagepack");
        }
        if (withContentTypeHeader == HeaderType.Valid)
        {
            httpRequestFeature.Headers.Append("Content-Type", MessagePackConstants.ContentType);
        }
        else if (withContentTypeHeader == HeaderType.Invalid)
        {
            httpRequestFeature.Headers.Append("Content-Type", "application/messagepack");
        }

        return HttpContext.Request;
    }
}