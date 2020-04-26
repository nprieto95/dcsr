using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Dcsr.ReplicationSender.DeltaHandler
{
    public static class GraphServiceClientFactory
    {
        public static GraphServiceClient CreateGraphClient(IConfiguration configuration)
        {
            IConfidentialClientApplication application;
            application = ConfidentialClientApplicationBuilder.Create(configuration["ClientId"])
                .WithAuthority(string.Format(configuration["AuthorityFormat"], configuration["TenantId"]))
                .WithClientSecret(configuration["ClientSecret"])
                .Build();

            return new GraphServiceClient(new ClientCredentialProvider(application));
        }
    }
}
