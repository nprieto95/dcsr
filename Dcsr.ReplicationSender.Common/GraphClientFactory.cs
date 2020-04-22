using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using static System.Configuration.ConfigurationManager;

namespace Dcsr.ReplicationSender
{
    public static class GraphClientFactory
    {
        public static GraphServiceClient CreateGraphClient()
        {
            IConfidentialClientApplication application;
            application = ConfidentialClientApplicationBuilder.Create(AppSettings["ClientId"])
                .WithAuthority(string.Format(AppSettings["AuthorityFormat"], AppSettings["TenantId"]))
                .WithClientSecret(AppSettings["ClientSecret"])
                .Build();

            return new GraphServiceClient(new ClientCredentialProvider(application));
        }
    }
}
