using FastEndpoints;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace Rudi.Dev.FastEndpoints.MessagePack.Internal;

public class MessagePackRequestBinder<TRequest> : RequestBinder<TRequest> where TRequest : notnull, new()
{
    private readonly ILogger<MessagePackRequestBinder<TRequest>> logger;
    private readonly MessagePackOptions options;

    public MessagePackRequestBinder(ILogger<MessagePackRequestBinder<TRequest>> logger, MessagePackOptions options)
    {
        this.logger = logger;
        this.options = options;
    }
    
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