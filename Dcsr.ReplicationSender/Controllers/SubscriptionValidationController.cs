using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Dcsr.ReplicationSender.Controllers
{
    public class SubscriptionValidationController : ApiController
    {

        public HttpResponseMessage Post([FromUri]string validationToken)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(validationToken, Encoding.Unicode);
            return response;
        }

    }
}
