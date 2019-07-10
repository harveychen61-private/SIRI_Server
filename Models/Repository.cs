using System.Collections.Generic;

namespace SIRI.Models
{
    public struct SiriSubscription{
        public AbstractSubscriptionStructure subscription;
        public StatusResponseStructure subscriptionResponse;

    } 


    public class Repository
    {
        public static IDictionary<int, Siri> Siris { get; set; }

        public static IDictionary<int, SiriSubscription> Subscriptions { get; set; }

        static Repository()
        {
            Siris = new Dictionary<int, Siri>();
            Subscriptions = new Dictionary<int, SiriSubscription>();

        }

    }

}