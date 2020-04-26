using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dcsr.ReplicationSender.DeltaHandler
{

    public class Functions
    {

        private IConfiguration configuration;

        public Functions(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task RenewSubscription([TimerTrigger("0 * * * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            var expirationDateTime = DateTime.Now.AddDays(3);
            var graphClient = GraphServiceClientFactory.CreateGraphClient(configuration);
            var subscriptionsRequestBuilder = graphClient.Subscriptions;
            var subscriptionsRequest = subscriptionsRequestBuilder.Request();
            var subscriptions = subscriptionsRequest.GetAsync().Result;
            var relevantSubscription = subscriptions.FirstOrDefault(s => s.NotificationUrl == configuration["NotificationUrl"]);
            var relevantSubscriptionId = relevantSubscription?.Id;
            if (relevantSubscriptionId != null)
            {
                relevantSubscription.ExpirationDateTime = expirationDateTime;
                await subscriptionsRequestBuilder[relevantSubscriptionId].Request().UpdateAsync(relevantSubscription);
            }
            else
                await subscriptionsRequest.AddAsync(
                    new Subscription
                    {
                        ChangeType = "updated",
                        NotificationUrl = configuration["NotificationUrl"],
                        ExpirationDateTime = expirationDateTime,
                        Resource = $"users('{configuration["TestUserId"]}')/drive/root"
                    });
        }

    }

}