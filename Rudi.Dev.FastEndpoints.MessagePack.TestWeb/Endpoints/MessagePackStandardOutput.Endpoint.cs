using FastEndpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackStandardOutputEndpoint : EndpointWithoutRequest<MessagePackStandardOutputResponse>
{
    public override void Configure()
    {
        Get("/mp-output-std");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new MessagePackStandardOutputResponse
        {
            Test = "Hello World!"
        };
        await this.SendAsMsgPackAsync(response, cancellation: ct);
    }
}