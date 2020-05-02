using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Linq;

namespace Dcsr.ReplicationSender.Functions
{

    public class SubscriptionRenewerService : ISubscriptionRenewerService
    {

        private readonly IConfiguration configuration;
        private readonly IGraphServiceClient graphServiceClient;

        public SubscriptionRenewerService(
            IConfiguration configuration,
            IGraphServiceClientFactory graphServiceClientFactory
        )
        {
            this.configuration = configuration;
            graphServiceClient = graphServiceClientFactory.CreateGraphClient();
        }

        public void RenewAllSubscriptions()
        {
            var expirationDateTime = DateTime.Now.AddDays(3);
            var subscriptionsRequestBuilder = graphServiceClient.Subscriptions;
            var subscriptionsRequest = subscriptionsRequestBuilder.Request();
            var subscriptions = subscriptionsRequest.GetAsync().Result;
            var relevantSubscription = subscriptions.FirstOrDefault(s => s.NotificationUrl == configuration["NotificationUrl"]);
            var relevantSubscriptionId = relevantSubscription?.Id;
            if (relevantSubscriptionId != null)
            {
                relevantSubscription.ExpirationDateTime = expirationDateTime;
                var result = subscriptionsRequestBuilder[relevantSubscriptionId].Request().UpdateAsync(relevantSubscription).Result;
            }
            else
            {
                var result = subscriptionsRequest.AddAsync(
                    new Subscription
                    {
                        ChangeType = "updated",
                        NotificationUrl = configuration["NotificationUrl"],
                        ExpirationDateTime = expirationDateTime,
                        Resource = $"users('{configuration["TestUserId"]}')/drive/root"
                    }
                ).Result;
            }
        }

    }

}