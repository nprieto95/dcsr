using System.Web.Http;
using static System.Configuration.ConfigurationManager;

namespace Dcsr.ReplicationSender
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.MapHttpAttributeRoutes();

            config.InitializeReceiveGraphWebHooks();

        }
    }

}