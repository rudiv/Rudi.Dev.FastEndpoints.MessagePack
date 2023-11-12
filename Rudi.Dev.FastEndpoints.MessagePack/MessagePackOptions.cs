using MessagePack;
using MessagePack.Resolvers;

namespace Rudi.Dev.FastEndpoints.MessagePack;

public class MessagePackOptions
{
    /// <summary>
    /// Defines whether the global MessagePack model binder should be added or not.
    /// Enabled by default, this can be disabled if you want to add the binder on a per request basis.
    /// </summary>
    public bool AddInputBinder { get; set; } = true;
    
    /// <summary>
    /// Defines the response header to use by default.
    /// Defaults to "application/msgpack".
    /// </summary>
    public string DefaultResponseHeader { get; set; } = MessagePackConstants.ContentType;

    /// <summary>
    /// The <see cref="IFormatterResolver"/> used to resolve formatters for types.
    /// Alternatively, set <see cref="SerializerOptions"/> directly.
    /// </summary>
    public IFormatterResolver Resolver
    {
        get => SerializerOptions.Resolver;
        set => SerializerOptions = new MessagePackSerializerOptions(value);
    }

    /// <summary>
    /// Sets the <see cref="MessagePackSerializerOptions"/> to be used.
    /// Alternatively, just set the <see cref="Resolver"/>.
    /// Defaults to using the <see cref="ContractlessStandardResolver"/>.
    /// </summary>
    public MessagePackSerializerOptions SerializerOptions { get; set; } = new(ContractlessStandardResolver.Instance);

    /// <summary>
    /// Defines whether a <see cref="MessagePackSerializationException"/> is thrown if the input is invalid.
    /// </summary>
    public bool ThrowOnInvalid { get; set; } = false;
}