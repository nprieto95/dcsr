using Dcsr.ReplicationSender.Receivers;
using Microsoft.AspNet.WebHooks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace Dcsr.ReplicationSender.Handlers
{

    public class GraphWebHookHandler : WebHookHandler
    {

        public GraphWebHookHandler()
        {
            Receiver = GraphWebHookReceiver.ReceiverName;
        }

        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            //Test user id: "8d1a86cb-986e-4655-a83c-58590b3b964c"
            return Task.CompletedTask;
        }

    }

}