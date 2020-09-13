namespace HappyEating.Campaign.Core.IntegrationEvent
{
    using System;

    public class CampaignUpdatedEvent
    {
        public string ProductId { get; set; } = "";

        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfUnit { get; set; }

        public string OrganizerId { get; set; } = "";

        public string OrganizationName { get; set; } = "";

        public DateTime CampaignBeginAt { get; set; }

        public DateTime CampaignEndAt { get; set; }

    }
}
