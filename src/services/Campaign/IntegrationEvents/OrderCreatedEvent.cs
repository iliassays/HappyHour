namespace HappyEating.Campaign.Core.IntegrationEvent
{
    using System;

    public class OrderCreatedEvent
    {
        public string OrderId { get; set; }

        public string CampaignId { get; set; }

        public int NumberOfOrderedUnit { get; set; }

        public string OrganizerId { get; set; } = "";

        public string OrganizationName { get; set; } = "";

    }
}
