using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Rudi.Dev.FastEndpoints.MessagePack.Internal;

public static class HttpRequestMsgPackExtensions
{
    /// <summary>
    /// Checks the Accept header for MessagePack content types.
    /// </summary>
    /// <returns>true if the Accept header contains a MessagePack content type; otherwise, false.</returns>
    public static bool AcceptsMsgPackContentType(this HttpRequest request)
    {
        var typedHeaders = request.GetTypedHeaders();
        return typedHeaders.Accept.Contains(MessagePackConstants.MediaTypeHeaderValue) ||
               typedHeaders.Accept.Contains(MessagePackConstants.XMediaTypeHeaderValue) ||
               typedHeaders.Accept.Contains(MessagePackConstants.VndMediaTypeHeaderValue);
    }
    
    /// <summary>
    /// Checks the Content-Type header for MessagePack types.
    /// </summary>
    /// <returns>true if the Content-Type header represents a MessagePack content type; otherwise, false.</returns>
    public static bool HasMsgPackContentType(this HttpRequest request)
    {
        return request.HasMsgPackContentType(out _);
    }

    private static bool HasMsgPackContentType(this HttpRequest request, out StringSegment charset)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!MediaTypeHeaderValue.TryParse(request.ContentType, out var mt))
        {
            charset = StringSegment.Empty;
            return false;
        }

        // Matches application/[x-|vnd-]msgpack
        if (mt.MediaType.Equals(MessagePackConstants.ContentType, StringComparison.OrdinalIgnoreCase) ||
            mt.MediaType.Equals(MessagePackConstants.XContentType, StringComparison.OrdinalIgnoreCase) ||
            mt.MediaType.Equals(MessagePackConstants.VndContentType, StringComparison.OrdinalIgnoreCase))
        {
            charset = mt.Charset;
            return true;
        }

        charset = StringSegment.Empty;
        return false;
    }
}