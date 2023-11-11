using System.Collections;
using System.Reflection;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rudi.Dev.FastEndpoints.MessagePack.Internal;

namespace Rudi.Dev.FastEndpoints.MessagePack;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MessagePack support to the FastEndpoints application.
    /// </summary>
    /// <remarks>
    /// Must be called before app.UseFastEndpoints().
    /// </remarks>
    /// <example>
    /// app.AddMessagePackBinding(o => o.Resolver = StandardResolver.Instance);
    /// app.Services.AddFastEndpoints();
    /// </example>
    /// <param name="configure">Action to configure the binder.</param>
    /// <returns>Service Collection.</returns>
    public static IServiceCollection AddMessagePackBinding(this IServiceCollection services, Action<MessagePackOptions>? configure = null)
    {
        var options = new MessagePackOptions();
        configure?.Invoke(options);

        services.TryAddSingleton<MessagePackOptions>(options);
        
        if (options.AddInputBinder)
        {
            services.TryAddSingleton(typeof(IRequestBinder<>), typeof(MessagePackRequestBinder<>));
        }

        return services;
    }
    
    public static EndpointDefinition ConfigureInboundMessagePack(this EndpointDefinition endpointDefinition)
    {
        endpointDefinition.Description(o =>
            o.Accepts(
                endpointDefinition.ReqDtoType,
                MessagePackConstants.ContentType,
                MessagePackConstants.XContentType,
                MessagePackConstants.VndContentType));
        return endpointDefinition;
    }

    /*
    /// <summary>
    /// Remaps the endpoints to accept inbound MessagePack.
    ///
    /// This must be called after .UseFastEndpoints.
    /// </summary>
    /// <remarks>
    /// This relies on internals of Minimal APIs, and may break with a future version of .NET.
    /// </remarks>
    /// <example>
    /// app.UseFastEndpoints();
    /// app.UseEndpointsWithMessagePack();
    /// </example>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder UseEndpointsWithMessagePack(this IEndpointRouteBuilder builder)
    {
        var datasource = builder.DataSources.First();
        // Find the _routeEntries field
        var routeEntriesField = datasource.GetType().GetField("_routeEntries", BindingFlags.NonPublic | BindingFlags.Instance);
        var routeEntries = (IList)routeEntriesField!.GetValue(datasource)!;
        // Now we need to add the "Accepts" convention to the endpoint, as FE doesn't support this properly yet.
        var routeEntryConventionsProperty = routeEntries[0]!.GetType().GetProperty("Conventions", BindingFlags.Public | BindingFlags.Instance);
        foreach (var routeEntry in routeEntries)
        {
            var conventions = (ICollection<Action<EndpointBuilder>>)routeEntryConventionsProperty!.GetValue(routeEntry)!;
            conventions.Add(b => b.Metadata.Add(new AcceptsMetadata(new []
            {
                MessagePackConstants.ContentType,
                MessagePackConstants.XContentType,
                MessagePackConstants.VndContentType
            })));
        }
        return builder;
    }*/
}