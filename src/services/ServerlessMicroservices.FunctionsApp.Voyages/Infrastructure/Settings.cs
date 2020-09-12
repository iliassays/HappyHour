using System;

namespace ServerlessMicroservices.HappyEating.Core
{
    public class Settings
    {
        private const string EnableAuthKey = "EnableAuth";

        // Storage
        private const string StorageAccountKey = "AzureWebJobsStorage";

        private const string InsightsInstrumentationKey = "InsightsInstrumentationKey";

        // Event Grid
        private const string CampaignExternalizationsEventGridTopicUrl = "CampaignExternalizationsEventGridTopicUrl";
        private const string CampaignExternalizationsEventGridTopicApiKey = "CampaignExternalizationsEventGridTopicApiKey";

        private const string OrderExternalizationsEventGridTopicUrl = "OrderExternalizationsEventGridTopicUrl";
        private const string OrderExternalizationsEventGridTopicApiKey = "OrderExternalizationsEventGridTopicApiKey";

        public bool EnableAuth()
        {
            if (
                GetEnvironmentVariable(EnableAuthKey) != null &&
                !string.IsNullOrEmpty(GetEnvironmentVariable(EnableAuthKey).ToString()) &&
                GetEnvironmentVariable(EnableAuthKey).ToString().ToLower() == "true"
            )
            {
                return true;
            }

            return false;
        }

        public static string GetStorageAccount()
        {
            return GetEnvironmentVariable(StorageAccountKey);
        }

        public static string GetInsightsInstrumentationKey()
        {
            return GetEnvironmentVariable(InsightsInstrumentationKey);
        }

        // Event Grid Urls

        public static string GetCampaignExternalizationsEventGridTopicUrl()
        {
            return GetEnvironmentVariable(CampaignExternalizationsEventGridTopicUrl);
        }

        public static string GetCampaignExternalizationsEventGridTopicApiKey()
        {
            return GetEnvironmentVariable(CampaignExternalizationsEventGridTopicApiKey);
        }

        public static string GetOrderExternalizationsEventGridTopicUrl()
        {
            return GetEnvironmentVariable(OrderExternalizationsEventGridTopicUrl);
        }

        public static string GetOrderExternalizationsEventGridTopicApiKey()
        {
            return GetEnvironmentVariable(OrderExternalizationsEventGridTopicApiKey);
        }
        
        //*** PRIVATE ***//
        private static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}