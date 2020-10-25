using Microsoft.Azure.WebJobs;

namespace Dcsr.ReplicationSender.Functions
{
    public class RenewSubscription
    {

        private readonly ISubscriptionRenewerService subscriptionRenewerService;

        public RenewSubscription(ISubscriptionRenewerService subscriptionRenewerService)
        {
            this.subscriptionRenewerService = subscriptionRenewerService;
        }

        [FunctionName("RenewSubscription")]
        public void Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            subscriptionRenewerService.RenewAllSubscriptions();
        }

    }
}
