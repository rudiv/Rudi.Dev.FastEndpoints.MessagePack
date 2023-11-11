using FastEndpoints;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace Rudi.Dev.FastEndpoints.MessagePack.Internal;

public class MessagePackRequestBinder<TRequest>(ILogger<MessagePackRequestBinder<TRequest>> logger, MessagePackOptions options) : RequestBinder<TRequest> where TRequest : notnull, new()
{
    public override async ValueTask<TRequest> BindAsync(BinderContext ctx, CancellationToken cancellation)
    {
        if (ctx.HttpContext.Request.HasMsgPackContentType())
        {
            try
            {
                return await MessagePackSerializer.DeserializeAsync<TRequest>(ctx.HttpContext.Request.Body, options.SerializerOptions, cancellation);
            } catch (MessagePackSerializationException ex)
            {
                logger.Log(options.ThrowOnInvalid ? LogLevel.Error : LogLevel.Warning, ex, "Failed to deserialize MessagePack request");
                if (options.ThrowOnInvalid)
                {
                    throw;
                }
            }
        }
        
        return await base.BindAsync(ctx, cancellation);
    }
}