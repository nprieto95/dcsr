using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Dcsr.ReplicationSender.Functions
{
    public class Program
    {
        public static async Task Main()
        {

            var host =new HostBuilder()

                .ConfigureServices(sc =>
                {
                    sc.AddTransient<IGraphServiceClientFactory, GraphServiceClientFactory>();
                    sc.AddTransient<ISubscriptionRenewerService, SubscriptionRenewerService>();
                })

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