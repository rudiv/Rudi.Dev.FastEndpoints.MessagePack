using FastEndpoints;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackOutputEndpoint : EndpointWithoutRequest<MessagePackOutputResponse>
{
    public override void Configure()
    {
        Get("/output");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new MessagePackOutputResponse
        {
            Test = "Hello World!"
        };
        await this.SendAsMsgPackAsync(response, cancellation: ct);
    }
}