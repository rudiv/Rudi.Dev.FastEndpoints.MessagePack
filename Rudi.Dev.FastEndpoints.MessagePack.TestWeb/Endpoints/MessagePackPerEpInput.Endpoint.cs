using FastEndpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackPerEpInputEndpoint : Endpoint<MessagePackInputRequest, MessagePackOutputResponse>
{
    public override void Configure()
    {
        Post("/mp-input-pe");
        AllowAnonymous();
        Description(o => o.Accepts<MessagePackInputRequest>(MessagePackConstants.ContentType));
    }

    public override async Task HandleAsync(MessagePackInputRequest req, CancellationToken ct)
    {
        await this.SendAsMsgPackAsync(new MessagePackInputResponse
        {
            PackedAt = DateOnly.FromDateTime(DateTime.Today),
            Test = req.Test
        });
    }
}