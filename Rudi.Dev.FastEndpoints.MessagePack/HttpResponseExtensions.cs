using FastEndpoints;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;

namespace Rudi.Dev.FastEndpoints.MessagePack;

public static class HttpResponseExtensions
{
    /// <summary>
    /// Send the response data formatted by MessagePack, regardless of the Accept header.
    /// </summary>
    /// <param name="response">The HttpResponse.</param>
    /// <param name="data">Response data.</param>
    /// <param name="serializerOptions">Message Pack Serializer Options.</param>
    /// <param name="cancellation">Cancellation Token.</param>
    /// <typeparam name="TResponse">Response type for <see cref="data"/>.</typeparam>
    /// <exception cref="InvalidOperationException">Invalid MessagePack configuration.</exception>
    public static async Task WriteAsMsgPackAsync<TResponse>(this HttpResponse response, TResponse data, MessagePackSerializerOptions? serializerOptions = null, CancellationToken cancellation = default)
    {
        response.HttpContext.MarkResponseStart();
        var options = response.HttpContext.RequestServices.GetService<MessagePackOptions>() ??
                    throw new InvalidOperationException("You must call app.UseMessagePackBinding() or pass your own MessagePackSerializerOptions.");
        serializerOptions ??= options.SerializerOptions;

        if (!cancellation.CanBeCanceled)
        {
            cancellation = response.HttpContext.RequestAborted;
        }

        response.ContentType = options.DefaultResponseHeader;
        await MessagePackSerializer.SerializeAsync(response.Body, data, serializerOptions, cancellation);
    }

    /// <summary>
    /// Send the response data formatted by MessagePack if the client accepts it.
    /// </summary>
    /// <param name="response">The HttpResponse.</param>
    /// <param name="data">Response data.</param>
    /// <param name="serializerOptions">Message Pack Serializer Options.</param>
    /// <param name="cancellation">Cancellation Token.</param>
    /// <typeparam name="TResponse">Response type for <see cref="data"/>.</typeparam>
    public static async Task WriteWithMsgPackAsync<TResponse>(this HttpResponse response, TResponse data, MessagePackSerializerOptions? serializerOptions = null,
        CancellationToken cancellation = default)
    {
        if (response.HttpContext.Request.AcceptsMsgPackContentType())
        {
            await response.WriteAsMsgPackAsync(data, serializerOptions, cancellation);
        }
        else
        {
            await response.SendAsync(data, cancellation: cancellation);
        }
    }
}