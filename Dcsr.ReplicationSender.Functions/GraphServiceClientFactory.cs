using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Dcsr.ReplicationSender.Functions
{

    public class GraphServiceClientFactory : IGraphServiceClientFactory
    {

        private readonly IConfiguration configuration;

        public GraphServiceClientFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public GraphServiceClient CreateGraphClient()
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