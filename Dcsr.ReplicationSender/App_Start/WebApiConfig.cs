using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using static System.Configuration.ConfigurationManager;

namespace Dcsr.ReplicationSender
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "SubscriptionValidation",
                routeTemplate: "api/webhooks/incoming/graph",
                defaults: new { controller = "SubscriptionValidation" },
                constraints: new
                {
                    validationTokenPresent = new QueryStringParameterPresentConstraint("validationToken"),
                    codeIsValid = new QueryStringParameterValueConstraint(parameterName: "code", parameterValue: AppSettings["MS_WebHookReceiverSecret_Graph"])
                }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.InitializeReceiveGraphWebHooks();

        }
    }

}