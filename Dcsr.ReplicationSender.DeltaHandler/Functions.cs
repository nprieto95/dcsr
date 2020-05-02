using Microsoft.Azure.WebJobs;

namespace Dcsr.ReplicationSender.Functions
{

    public class Functions
    {

        public static void RenewSubscription([TimerTrigger("0 * * * * *", RunOnStartup = true)] TimerInfo timerInfo, ISubscriptionRenewerService subscriptionRenewerService)
        {
            subscriptionRenewerService.RenewAllSubscriptions();
        }

    }

}