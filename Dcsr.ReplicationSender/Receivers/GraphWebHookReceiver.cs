using Microsoft.AspNet.WebHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace Dcsr.ReplicationSender.Receivers
{
    public class GraphWebHookReceiver : WebHookReceiver
    {

        internal const string RecName = "graph";

        public static string ReceiverName => RecName;

        public override string Name => RecName;

        public override Task<HttpResponseMessage> ReceiveAsync(string id, HttpRequestContext context, HttpRequestMessage request)
        {
            return ExecuteWebHookAsync(id, context, request, new string[] { }, request.Content.ReadAsStringAsync());
        }

    }
}