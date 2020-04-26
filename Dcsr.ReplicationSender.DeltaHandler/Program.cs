using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Dcsr.ReplicationSender.DeltaHandler
{
    public class Program
    {

        public static async Task Main()
        {

            var host =new HostBuilder()

                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddEnvironmentVariables();
                })

                .ConfigureWebJobs(wjb =>
                {
                    wjb.AddAzureStorageCoreServices();
                    wjb.AddTimers();
                    wjb.AddAzureStorage();
                })

                .Build();

            using (host)
            {
                await host.RunAsync();
            }

        }

    }
}