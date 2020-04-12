using Microsoft.AspNet.WebHooks;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Dcsr.ReplicationSender.Receivers
{
    public class GraphWebHookReceiver : WebHookReceiver
    {

        internal const string RecName = "graph";

        public static string ReceiverName => RecName;

        public override string Name => RecName;

        public async override Task<HttpResponseMessage> ReceiveAsync(string id, HttpRequestContext context, HttpRequestMessage request)
        {
            if (!request.GetQueryNameValuePairs().Any(kvp => kvp.Key=="validationToken"))
            {
                return await ExecuteWebHookAsync(id, context, request, new string[] { }, await request.Content.ReadAsStringAsync());
            }
            else
            {
                return WebHookVerification(request);
            }
        }

        protected virtual HttpResponseMessage WebHookVerification(HttpRequestMessage request)
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(request.RequestUri.ParseQueryString()["validationToken"], Encoding.Unicode);
            return response;
        }

    }
}