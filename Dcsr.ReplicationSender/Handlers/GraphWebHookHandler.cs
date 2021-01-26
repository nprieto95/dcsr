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

        private static Dictionary<string, (string path, string name)> fileIdentityTracking = new Dictionary<string, (string, string)>();

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
            if (result.Count != 0)
            {
                foreach (var item in result)
                {
                    if (item.Deleted == null)
                        if (!fileIdentityTracking.ContainsKey(item.Id))
                            fileIdentityTracking.Add(item.Id, (item.ParentReference.Path, item.Name));
                        else
                        {
                            var newPath = item.ParentReference.Path;
                            var newName = item.Name;
                            if (newPath != fileIdentityTracking[item.Id].path)
                                item.ParentReference.Path = $"{fileIdentityTracking[item.Id].path}*{newPath}";
                            if (newName != fileIdentityTracking[item.Id].name)
                                item.Name = $"{fileIdentityTracking[item.Id].name}*{newName}";
                            fileIdentityTracking[item.Id] = (newPath, newName);
                        }
                    else
                    {
                        item.ParentReference.Path = fileIdentityTracking[item.Id].path;
                        item.Name = fileIdentityTracking[item.Id].name;
                    }
                }
                await deltaQueueClient.SendMessageAsync(JsonConvert.SerializeObject(result));
            }
            deltaLink = (string)result.AdditionalData["@odata.deltaLink"];
        }

    }

}