using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Dcsr.ReplicationSender.Functions.FunctionAppStartup))]
namespace Dcsr.ReplicationSender.Functions
{
    public class FunctionAppStartup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IGraphServiceClientFactory, GraphServiceClientFactory>();
            builder.Services.AddTransient<ISubscriptionRenewerService, SubscriptionRenewerService>();
        }
    }
}
