using MessagePack;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

[MessagePackObject]
public class MessagePackStandardInputResponse
{
    [Key(0)]
    public DateOnly PackedAt { get; set; }
    [Key(1)]
    public string Test { get; set; }
}