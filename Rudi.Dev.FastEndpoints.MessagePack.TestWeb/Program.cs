using System.Runtime.CompilerServices;
using FastEndpoints;
using Rudi.Dev.FastEndpoints.MessagePack;

[assembly: InternalsVisibleTo("Rudi.Dev.FastEndpoints.MessagePack.Tests")]

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (builder.Environment.IsEnvironment("Global"))
{
    app.UseFastEndpoints(o => o.Endpoints.Configurator = ep => ep.ConfigureInboundMessagePack());
}
else
{
    app.UseFastEndpoints();
}

app.Run();


public partial class Program { }