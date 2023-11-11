using FastEndpoints;
using MessagePack;

namespace Rudi.Dev.FastEndpoints.MessagePack;

public static class EndpointExtensions
{
    /// <summary>
    /// Send the response data formatted by MessagePack, regardless of the Accept header.
    /// </summary>
    /// <param name="data">Response data.</param>
    /// <param name="serializerOptions">Message Pack Serializer Options.</param>
    /// <param name="cancellation">Cancellation Token.</param>
    /// <typeparam name="TResponse">Response type for <see cref="data"/>.</typeparam>
    public static Task SendAsMsgPackAsync<TResponse>(this BaseEndpoint endpoint, TResponse data, MessagePackSerializerOptions? serializerOptions = null, CancellationToken cancellation = default)
    {
        return endpoint.HttpContext.Response.WriteAsMsgPackAsync(data, serializerOptions, cancellation);
    }
    
    /// <summary>
    /// Send the response data formatted by MessagePack if the client accepts it.
    /// </summary>
    /// <param name="data">Response data.</param>
    /// <param name="serializerOptions">Message Pack Serializer Options.</param>
    /// <param name="cancellation">Cancellation Token.</param>
    /// <typeparam name="TResponse">Response type for <see cref="data"/>.</typeparam>
    public static Task SendWithMsgPackAsync<TResponse>(this BaseEndpoint endpoint, TResponse data, MessagePackSerializerOptions? serializerOptions = null, CancellationToken cancellation = default)
    {
        return endpoint.HttpContext.Response.WriteWithMsgPackAsync(data, serializerOptions, cancellation);
    }
}