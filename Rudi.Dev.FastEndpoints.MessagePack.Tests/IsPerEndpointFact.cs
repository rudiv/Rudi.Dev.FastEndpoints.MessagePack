namespace Rudi.Dev.FastEndpoints.MessagePack.Tests;

public class IsPerEndpointFact : FactAttribute
{
    public IsPerEndpointFact(bool isPerEp = false)
    {
        var envSet = Environment.GetEnvironmentVariable("PEREP") != null;
        if ((!envSet && isPerEp) || (envSet && !isPerEp))
        {
            Skip = $"Not running this test based on environment.";
        }
    }
}