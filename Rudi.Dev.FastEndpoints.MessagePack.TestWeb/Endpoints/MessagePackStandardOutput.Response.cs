using MessagePack;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

[MessagePackObject]
public class MessagePackStandardOutputResponse
{
    [Key(0)]
    public string Test { get; set; }
}