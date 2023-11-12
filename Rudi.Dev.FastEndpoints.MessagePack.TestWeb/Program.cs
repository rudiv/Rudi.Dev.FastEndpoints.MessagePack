using System.Runtime.CompilerServices;
using FastEndpoints;
using Rudi.Dev.FastEndpoints.MessagePack;
using Rudi.Dev.FastEndpoints.MessagePack.TestWeb;

[assembly: InternalsVisibleTo("Rudi.Dev.FastEndpoints.MessagePack.Tests")]

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Services.GetService<ShouldBeGlobal>() != null)
{
    app.UseFastEndpoints(o =>
    {
        o.Serializer.ResponseSerializer = FastEndpointsResponseSerializer.MessagePack;
        o.Endpoints.Configurator = ep => ep.ConfigureInboundMessagePack();
    });
}
else
{
    app.UseFastEndpoints();
}

app.Run();


public partial class Program { }