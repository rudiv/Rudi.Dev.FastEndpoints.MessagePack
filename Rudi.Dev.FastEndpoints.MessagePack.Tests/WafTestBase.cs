using FastEndpoints;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb;

// Force the tests to run sequentially so the FE internal statics don't compete with each other on Initialisation
[assembly: CollectionBehavior(collectionBehavior: CollectionBehavior.CollectionPerClass, DisableTestParallelization = true)]

namespace Rudi.Dev.FastEndpoints.MessagePack.Tests;

public abstract class WafTestBase : IAsyncLifetime
{
    public WebApplicationFactory<Program> App { get; private set; }
    public HttpClient Client { get; private set; }
    
    public Task InitializeAsync()
    {
        App = new WebApplicationFactory<Program>().WithWebHostBuilder(
            b =>
            {
                b.ConfigureLogging(l => l.ClearProviders().AddDebug());
                b.ConfigureTestServices(ConfigureServices);
            });
        Client = App.CreateClient();
        return Task.CompletedTask;
    }

    public abstract void ConfigureServices(IServiceCollection services);

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await App.DisposeAsync();
    }
}

public class GlobalWafTest : WafTestBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddMessagePackBinding();
        services.AddFastEndpoints();
        services.AddSingleton<ShouldBeGlobal>();
    }
}
public class NoInputBinderWafTest : WafTestBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddMessagePackBinding(o => o.AddInputBinder = false);
        services.AddFastEndpoints();
        services.AddSingleton<ShouldBeGlobal>();
    }
}
public class PerEndpointWafTest : WafTestBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddMessagePackBinding();
        services.AddFastEndpoints();
    }
}
public class StandardResolverWafTest : WafTestBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddMessagePackBinding(o => o.Resolver = StandardResolver.Instance);
        services.AddFastEndpoints();
        services.AddSingleton<ShouldBeGlobal>();
    }
}