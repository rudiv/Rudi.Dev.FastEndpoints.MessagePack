using FastEndpoints;
using FastEndpoints.Testing;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests;

public abstract class BaseFixture : TestFixture<Program>
{
    private readonly string env = "Global";
    public BaseFixture(IMessageSink s, string env) : base(s)
    {
        this.env = env;
    }
    
    protected override void ConfigureServices(IServiceCollection s)
    {
        base.ConfigureServices(s);
        s.AddMessagePackBinding();
        s.AddFastEndpoints();
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        base.ConfigureApp(a);
        a.UseEnvironment(env);
    }
}

public class GlobalFixture : BaseFixture
{
    public GlobalFixture(IMessageSink s) : base(s, "Global") {}
}
public class PerEpFixture : BaseFixture
{
    public PerEpFixture(IMessageSink s) : base(s, "PerEp") {}
}
public class StandardResolverFixture : BaseFixture
{
    public StandardResolverFixture(IMessageSink s) : base(s, "Global") {}

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.AddMessagePackBinding(o => o.Resolver = StandardResolver.Instance);
        s.AddFastEndpoints();
    }
}