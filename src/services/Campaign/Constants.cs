namespace HappyEating.Campaign.Core
{
    public class Constants
    {
        public const string MONGO_CONNECTION_STRING = "MONGO_CONNECTION_STRING";
        public const string DATABASE_NAME = "DATABASE_NAME";
        public const string COLLECTION_NAME = "COLLECTION_NAME";

        public const string EVG_EVENT_ORDER_CREATED = "OrderCreated";
        public const string EVG_EVENT_ORDER_CANCELLED = "OrderCanceled";

        public const string EVG_EVENT_CAMPAIGN_CREATED = "CampaignCreated";
        public const string EVG_EVENT_CAMPAIGN_UPDATED = "CampaignUpdated";
        public const string EVG_EVENT_CAMPAIGN_FAILED = "CampaignFailed";

        public const string EVG_EVENT_CAMPAIGN_ORDER_ACCEPTED = "CampaignOrderAccepted";
        public const string EVG_EVENT_CAMPAIGN_ORDER_REJECTED = "CampaignOrderRejected";
    }
}