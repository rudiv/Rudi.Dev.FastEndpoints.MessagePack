using FastEndpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackVariedOutputEndpoint : EndpointWithoutRequest<MessagePackOutputResponse>
{
    public override void Configure()
    {
        Get("/vary-output");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new MessagePackOutputResponse
        {
            Test = "Hello World!"
        };
        await this.SendWithMsgPackAsync(response, cancellation: ct);
    }
}