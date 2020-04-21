using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GraphDeltaRequestPoc
{
    class Program
    {
        static void Main(string[] args)
        {

            string AuthorityFormat = "";

            IConfidentialClientApplication application;
            application = ConfidentialClientApplicationBuilder.Create()
                .WithAuthority(string.Format(AuthorityFormat, ""))//(AadAuthorityAudience.AzureAdMultipleOrgs)//
                //.WithRedirectUri("http://localhost:53778/Account/GrantPermissions")
                .WithClientSecret(@"")
                .Build();

            var graphClient = new GraphServiceClient(new ClientCredentialProvider(application));

            var request = graphClient.Users[].Drive.Root.Delta().Request();

            var result = request.GetAsync().Result;

            Debugger.Break();

            //Console.WriteLine(JsonConvert.SerializeObject(result));

        }
    }
}
