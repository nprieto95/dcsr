using Azure.Storage.Queues;
using Dcsr.ReplicationSender.Receivers;
using Microsoft.AspNet.WebHooks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Dcsr.ReplicationSender.Handlers
{

    public class GraphWebHookHandler : WebHookHandler
    {

        private static string deltaLink = null;

        public GraphWebHookHandler()
        {
            Receiver = GraphWebHookReceiver.ReceiverName;
        }

        public async override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            IConfidentialClientApplication application = ConfidentialClientApplicationBuilder.Create(ConfigurationManager.AppSettings["ClientId"]).WithAuthority(string.Format(ConfigurationManager.AppSettings["AuthorityFormat"], (object)ConfigurationManager.AppSettings["TenantId"])).WithClientSecret(ConfigurationManager.AppSettings["ClientSecret"]).Build();
            GraphServiceClient graphClient = new GraphServiceClient(new ClientCredentialProvider(application));
            IDriveItemDeltaRequest request;
            if (string.IsNullOrWhiteSpace(deltaLink))
            {
                request = graphClient.Sites.Root.Drive.Root.Delta().Request();
            }
            else
            {
                request = new DriveItemDeltaRequest(deltaLink, graphClient, new List<Option>());
            }
            IDriveItemDeltaCollectionPage result = await request.GetAsync();
            QueueServiceClient queueClient = new QueueServiceClient(ConfigurationManager.AppSettings["Storage"]);
            QueueClient deltaQueueClient = queueClient.GetQueueClient("deltas");
            await deltaQueueClient.CreateIfNotExistsAsync();
            await deltaQueueClient.SendMessageAsync(JsonConvert.SerializeObject(result));
            deltaLink = (string)result.AdditionalData["@odata.deltaLink"];
        }

    }

}