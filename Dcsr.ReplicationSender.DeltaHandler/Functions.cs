using Microsoft.Azure.WebJobs;
using System.IO;
using static System.Configuration.ConfigurationManager;
using System.Linq;

namespace Dcsr.ReplicationSender.DeltaHandler
{
    public class Functions
    {
        
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        public async static void RenewSubscription([TimerTrigger("0 0 0 */1.5 * *")] TimerInfo timerInfo)
        {
            var graphClient = GraphClientFactory.CreateGraphClient();
            var subscriptionsRequest = graphClient.Users[AppSettings["TestUserId"]].Drive.Root.Subscriptions.Request();
            var subscriptions = await subscriptionsRequest.GetAsync();
            var relevantSubscriptionId = subscriptions.FirstOrDefault(s => s.NotificationUrl == AppSettings["NotificationUrl"])?.Id;
            if(relevantSubscriptionId != null)
                graphClient.Subscriptions[relevantSubscriptionId].Request().UpdateAsync()
        }

    }
}