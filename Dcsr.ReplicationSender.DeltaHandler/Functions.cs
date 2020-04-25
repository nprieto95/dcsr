using Microsoft.Azure.WebJobs;
using System.IO;
using static System.Configuration.ConfigurationManager;
using System.Linq;
using System;
using Microsoft.Graph;

namespace Dcsr.ReplicationSender.DeltaHandler
{
    public class Functions
    {
        
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}

        public async static void RenewSubscription([TimerTrigger("*/5 0 0 * * *")] TimerInfo timerInfo)
        {
            var expirationDateTime = DateTime.Now.AddDays(3);
            var graphClient = GraphClientFactory.CreateGraphClient();
            var subscriptionsRequest = graphClient.Users[AppSettings["TestUserId"]].Drive.Root.Subscriptions.Request();
            var relevantSubscription = (await subscriptionsRequest.GetAsync()).FirstOrDefault(s => s.NotificationUrl == AppSettings["NotificationUrl"]);
            var relevantSubscriptionId = relevantSubscription?.Id;
            if (relevantSubscriptionId != null)
            {
                relevantSubscription.ExpirationDateTime = expirationDateTime;
                await graphClient.Subscriptions[relevantSubscriptionId].Request().UpdateAsync(relevantSubscription);
            }
            else
            {
                await subscriptionsRequest.AddAsync(
                    new Subscription
                    {
                        ChangeType = "update",
                        NotificationUrl = AppSettings["NotificationUrl"],
                        ExpirationDateTime = expirationDateTime
                    }
                ); ;
            }
        }

    }
}