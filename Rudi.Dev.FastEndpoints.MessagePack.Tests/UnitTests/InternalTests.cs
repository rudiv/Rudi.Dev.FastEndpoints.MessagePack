using MessagePack.Resolvers;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests.UnitTests;

// Just to get that 100% coverage score
public class InternalTests
{
    [Fact]
    public void CanGetResolver()
    {
        var opts = new MessagePackOptions()
        {
            Resolver = StandardResolver.Instance
        };
        Assert.Equal(StandardResolver.Instance, opts.Resolver);
    }

    [Fact]
    public void SerializerOverride_FunctionsNullDto()
    {
        Assert.Equal(Task.CompletedTask, FastEndpointsResponseSerializer.MessagePack(null!, null, null!, null, CancellationToken.None));
    }
}