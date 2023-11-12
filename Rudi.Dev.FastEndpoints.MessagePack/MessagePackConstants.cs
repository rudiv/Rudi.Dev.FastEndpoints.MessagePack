using Microsoft.Net.Http.Headers;

namespace Rudi.Dev.FastEndpoints.MessagePack;

public static class MessagePackConstants
{
    public const string ContentType = "application/msgpack";
    public const string XContentType = "application/x-msgpack";
    public const string VndContentType = "application/vnd.msgpack";
    
    public static readonly MediaTypeHeaderValue MediaTypeHeaderValue = new(ContentType);
    public static readonly MediaTypeHeaderValue XMediaTypeHeaderValue = new(XContentType);
    public static readonly MediaTypeHeaderValue VndMediaTypeHeaderValue = new(VndContentType);
}