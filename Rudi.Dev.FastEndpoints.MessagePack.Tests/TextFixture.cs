using Bogus;
using FastEndpoints;
using FastEndpoints.Testing;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests;

public abstract class TestFixtureX<TProgram> : BaseFixture, IAsyncLifetime, IFixture where TProgram : class
{
    /// <inheritdoc />
    /// >
    public Faker Fake => _faker;

    /// <summary>
    /// the service provider of the bootstrapped web application
    /// </summary>
    public IServiceProvider Services => _app.Services;

    /// <summary>
    /// the test server of the underlying <see cref="WebApplicationFactory{TEntryPoint}" />
    /// </summary>
    public TestServer Server => _app.Server;

    /// <summary>
    /// the default http client
    /// </summary>
    public HttpClient Client { get; set; }

    readonly WebApplicationFactory<TProgram> _app;

    protected TestFixtureX(IMessageSink s)
    {
        _app = new WebApplicationFactory<TProgram>().WithWebHostBuilder(
        b =>
        {
            b.UseEnvironment("Testing");
            b.ConfigureLogging(l => l.ClearProviders().AddXUnit(s));
            b.ConfigureTestServices(ConfigureServices);
            ConfigureApp(b);
        });
        Client = _app.CreateClient();
    }

    /// <summary>
    /// override this method if you'd like to do some one-time setup for the test-class.
    /// it is run before any of the test-methods of the class is executed.
    /// </summary>
    protected virtual Task SetupAsync()
        => Task.CompletedTask;

    /// <summary>
    /// override this method if you'd like to do some one-time teardown for the test-class.
    /// it is run after all test-methods have executed.
    /// </summary>
    protected virtual Task TearDownAsync()
        => Task.CompletedTask;

    /// <summary>
    /// override this method if you'd like to provide any configuration for the web host of the underlying <see cref="WebApplicationFactory{TEntryPoint}" />
    /// />
    /// </summary>
    protected virtual void ConfigureApp(IWebHostBuilder a) { }

    /// <summary>
    /// override this method if you'd like to override (remove/replace) any services registered in the underlying web application's DI container.
    /// </summary>
    protected virtual void ConfigureServices(IServiceCollection s) { }

    /// <summary>
    /// create a client for the underlying web application
    /// </summary>
    /// <param name="o">optional client options for the WAF</param>
    public HttpClient CreateClient(WebApplicationFactoryClientOptions? o = null)
        => CreateClient(_ => { }, o);

    /// <summary>
    /// create a client for the underlying web application
    /// </summary>
    /// <param name="c">configuration action for the client</param>
    /// <param name="o">optional client options for the WAF</param>
    public HttpClient CreateClient(Action<HttpClient> c, WebApplicationFactoryClientOptions? o = null)
    {
        var client = o is null ? _app.CreateClient() : _app.CreateClient(o);
        c(client);

        return client;
    }

    /// <summary>
    /// create a http message handler for the underlying web host/test server
    /// </summary>
#if NET7_0_OR_GREATER
    public HttpMessageHandler CreateHandler(Action<HttpContext>? c = null)
        => c is null ? _app.Server.CreateHandler() : _app.Server.CreateHandler(c);
#else
    public HttpMessageHandler CreateHandler()
        => _app.Server.CreateHandler();
#endif

    public Task InitializeAsync()
        => SetupAsync();

    public virtual async Task DisposeAsync()
    {
        Client.Dispose();
        _app.Server.Dispose();
        _app.Dispose();
        await _app.DisposeAsync();

        await TearDownAsync();
    }
}

public abstract class BaseFixtureX : TestFixtureX<Program>
{
    private readonly string env = "Global";
    public BaseFixtureX(IMessageSink s, string env) : base(s)
    {
        this.env = env;
    }
    
    protected override void ConfigureServices(IServiceCollection s)
    {
        base.ConfigureServices(s);
        s.AddMessagePackBinding();
        s.AddFastEndpoints();
        s.AddSingleton<ShouldBeGlobal>();
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        base.ConfigureApp(a);
        a.UseEnvironment(env);
    }
}

public class GlobalFixture : BaseFixtureX
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
public class PerEpFixture : BaseFixtureX
{
    public PerEpFixture(IMessageSink s) : base(s, "PerEp") {}
    
    protected override void ConfigureServices(IServiceCollection s)
    {
        s.AddMessagePackBinding(o => o.Resolver = ContractlessStandardResolver.Instance);
        s.AddFastEndpoints();
    }

    protected override Task SetupAsync()
    {
        Client.BaseAddress = new Uri("http://localhost:5010");
        return base.SetupAsync();
    }
}
public class StandardResolverFixture : BaseFixtureX
{
    public StandardResolverFixture(IMessageSink s) : base(s, "Global") {}

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.AddMessagePackBinding(o => o.Resolver = StandardResolver.Instance);
        s.AddFastEndpoints();
        s.AddSingleton<ShouldBeGlobal>();
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