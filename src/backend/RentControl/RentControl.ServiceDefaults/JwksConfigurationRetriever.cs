using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace RentControl.ServiceDefaults;

internal sealed class JwksConfigurationRetriever
    : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(
        string address,
        IDocumentRetriever retriever,
        CancellationToken cancellationToken)
    {
        var document = await retriever.GetDocumentAsync(
            address,
            cancellationToken);
        var jwks = new JsonWebKeySet(document);
        var configuration = new OpenIdConnectConfiguration();

        foreach (var key in jwks.GetSigningKeys())
        {
            configuration.SigningKeys.Add(key);
        }

        return configuration;
    }
}
