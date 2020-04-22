using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Dcsr.ReplicationSender.DeltaHandler
{

    class Program
    {

        static async Task Main()
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
            });
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }

    }

}
