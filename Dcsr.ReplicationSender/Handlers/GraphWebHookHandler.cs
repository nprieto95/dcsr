using Azure.Storage.Queues;
using Dcsr.ReplicationSender.Receivers;
using Microsoft.AspNet.WebHooks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;

namespace Dcsr.ReplicationSender.Handlers
{

    public class GraphWebHookHandler : WebHookHandler
    {

        private static IDriveItemDeltaRequest nextRequest = null;

        public GraphWebHookHandler()
        {
            Receiver = GraphWebHookReceiver.ReceiverName;
        }

        public async override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            IConfidentialClientApplication application = ConfidentialClientApplicationBuilder.Create(ConfigurationManager.AppSettings["ClientId"]).WithAuthority(string.Format(ConfigurationManager.AppSettings["AuthorityFormat"], (object)ConfigurationManager.AppSettings["TenantId"])).WithClientSecret(ConfigurationManager.AppSettings["ClientSecret"]).Build();
            GraphServiceClient graphClient = new GraphServiceClient(new ClientCredentialProvider(application));
            IDriveItemDeltaRequest request = nextRequest ?? graphClient.Sites.Root.Drive.Root.Delta().Request();
            IDriveItemDeltaCollectionPage result = await request.GetAsync();
            QueueServiceClient queueClient = new QueueServiceClient(ConfigurationManager.AppSettings["Storage"]);
            QueueClient deltaQueueClient = queueClient.GetQueueClient("deltas");
            await deltaQueueClient.CreateIfNotExistsAsync();
            await deltaQueueClient.SendMessageAsync(JsonConvert.SerializeObject(result));
            nextRequest = result.NextPageRequest;
        }

    }

}