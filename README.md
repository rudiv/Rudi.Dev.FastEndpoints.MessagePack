# Rudi.Dev.FastEndpoints.MessagePack

Add MessagePack Support to your FastEndpoints.

## Why

- Because MessagePack

## Will it work with any FE project

Yup, most likely.

## Usage

Add `Rudi.Dev.FastEndpoints.MessagePack` from NuGet.

To add support for input bindings globally, you need to call `.AddMessagePackBinding()` before `.AddFastEndpoints()`, and add `.ConfigureInboundMessagePack()` to a global configurator within FastEndpoints.

For example:
```csharp
builder.Services
    .AddMessagePackBinding()
    .AddFastEndpoints();

// ...

app.UseFastEndpoints(c => {
    c.Endpoints.Configurator = ep => ep.ConfigureInboundMessagePack();
});
```

To enable support on a per-endpoint basis instead, don't setup the global configurator. Instead, configure your endpoint as follows:

```csharp
public override void Configure()
{
    Description(o => o.Accepts<MyRequestObject>("application/x-msgpack"));
    
    // ..
}
```

To receive content at your endpoint, you need to set the `Content-Type` header of the request to `application/msgpack`, `application/x-msgpack` or `application/vnd-msgpack`. If you're using this, you're likely looking for performance, so use `application/msgpack` where possible as it will short-circuit quicker and save you approximately 4 attoseconds.

To send content from your endpoint via MessagePack, use `this.SendAsMsgPackAsync(dto)`. If you prefer to use content-negotiation (ie. the `Accept` header must be one of the above), use `this.SendWithMsgPackAsync(dto)`.

For example:
```csharp
public override Task HandleAsync(CancellationToken ct)
{
    var myDto = new MyResponse { Something = "MessagePack Me!" };
    return this.SendAsMsgPackAsync(myDto); // or SendWithMsgPackAsync(myDto);
}
```

You can also override the default Response Serializer of FastEndpoints to work with content-negotiation by default whilst you retain use of `SendAsync`, as follows:

```csharp
// Program
app.UseFastEndpoints(c => c.Serializer.ResponseSerializer = FastEndpointsResponseSerializer.MessagePack);

// Endpoint
return SendAsync(myDto); // will send as MessagePack if available in Accept header, or JSON otherwise
```

To override the MessagePack serializer options, for example if you wish to enable compression, just change it in the options:

```csharp
builder.AddMessagePackBinding(o =>
    {
        o.SerializerOptions = new MessagePackSerializerOptions(StandardResolver.Instance)
            .WithCompression(MessagePackCompression.Lz4BlockArray);
    });
```

If you need to change the response header (for example for compatibility with another library that only reads `application/x-msgpack`), you can do this with:

```csharp
builder.AddMessagePackBinding(o => o.DefaultResponseHeader = MessagePackConstants.XContentType);
```


## Recommendations

By default, this uses MessagePack's Contractless resolver which allows you to change not very much (basically just the Program.cs) to get MessagePack serialization working.

Don't do this.

Instead, you should be decorating your DTOs with `[MessagePackObject]` and `[Key(..)]` attributes. This will allow you to use the `StandardResolver` which is more performant.

```csharp
// Program
builder.AddMessagePackBinding(o => o.Resolver = StandardResolver.Instance);

// DTO
[MessagePackObject]
public class MyRequestObject {
    [Key(0)]
    public string MyProperty { get; set; }
    [Key(1)]
    public int AnotherProperty { get; set; }
    [Key(2)]
    public DateTime ADateProperty { get; set; }
}
```

## What's supported

Everything, I think. If you think of anything, open up an issue / PR.

## Troubleshooting

### I'm getting a 415 Unsupported Media Type

Make sure you have set up the configurator correctly, like `c.Endpoints.Configurator = ep => ep.ConfigureInboundMessagePack();`.

Or wire the endpoint up manually as described above on a per-endpoint basis.

### I'm still getting JSON in my responses

Are you using `SendWithMsgPackAsync(..)`? If so, make sure you have `Accept: application/msgpack` in your request headers. Or use `SendAsMsgPackAsync(..)` to force it.