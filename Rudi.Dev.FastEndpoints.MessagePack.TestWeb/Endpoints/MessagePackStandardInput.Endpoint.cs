using FastEndpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackStandardInputEndpoint : Endpoint<MessagePackStandardInputRequest, MessagePackOutputResponse>
{
    public override void Configure()
    {
        Post("/mp-input-std");
        AllowAnonymous();
    }

    public override async Task HandleAsync(MessagePackStandardInputRequest req, CancellationToken ct)
    {
        await this.SendAsMsgPackAsync(new MessagePackStandardInputResponse
        {
            PackedAt = DateOnly.FromDateTime(DateTime.Today),
            Test = req.Test
        });
    }
}