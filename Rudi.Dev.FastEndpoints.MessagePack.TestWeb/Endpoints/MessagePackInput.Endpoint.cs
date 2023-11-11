using FastEndpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackInputEndpoint : Endpoint<MessagePackInputRequest, MessagePackOutputResponse>
{
    public override void Configure()
    {
        Post("/mp-input");
        AllowAnonymous();
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