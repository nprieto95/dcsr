using Dcsr.ReplicationSender.Receivers;
using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
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
            Trace.TraceInformation($"Received {context.Data}");
            context.Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            return Task.CompletedTask;
        }

    }

}