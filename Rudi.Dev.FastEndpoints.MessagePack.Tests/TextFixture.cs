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

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        base.ConfigureApp(a);
        a.ConfigureKestrel(k =>
        {
            k.ListenAnyIP(5009);
        });
    }

    protected override Task SetupAsync()
    {
        Client.BaseAddress = new Uri("http://localhost:5009");
        return base.SetupAsync();
    }
}
public class PerEpFixture : BaseFixture
{
    public PerEpFixture(IMessageSink s) : base(s, "PerEp") {}

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        a.UseEnvironment("PerEp");
        a.ConfigureKestrel(k =>
        {
            k.ListenAnyIP(5010);
        });
    }

    protected override Task SetupAsync()
    {
        Client.BaseAddress = new Uri("http://localhost:5010");
        return base.SetupAsync();
    }
}
public class StandardResolverFixture : BaseFixture
{
    public StandardResolverFixture(IMessageSink s) : base(s, "Global") {}

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.AddMessagePackBinding(o => o.Resolver = StandardResolver.Instance);
        s.AddFastEndpoints();
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        base.ConfigureApp(a);
        a.ConfigureKestrel(k =>
        {
            k.ListenAnyIP(5011);
        });
    }

    protected override Task SetupAsync()
    {
        Client.BaseAddress = new Uri("http://localhost:5011");
        return base.SetupAsync();
    }
}