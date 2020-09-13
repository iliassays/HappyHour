namespace HappyEating.Campaign.Core.IntegrationEvent
{
    public class CampaignOrderRejectedEvent
    {
        public string OrderId { get; set; }

        public int ActualOrderedUnit { get; set; }

        public int AvailableUnit { get; set; }

        public string FailReason { get; set; }
    }
}
