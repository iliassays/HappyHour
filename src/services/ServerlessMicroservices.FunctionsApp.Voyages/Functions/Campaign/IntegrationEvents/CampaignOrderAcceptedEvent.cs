namespace ServerlessMicroservices.FunctionsApp.HappyEating.Core.IntegrationEvent
{
    using System;

    public class CampaignOrderAcceptedEvent
    {
        public string OrderId { get; set; }

        public int ActualOrderedUnit { get; set; }

        public int AvailableUnit { get; set; }

    }
}
