// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=EVGH_TransactionExternalization2Voyages

namespace ServerlessMicroservices.FunctionsApp.HappyEating.Function
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using ServerlessMicroservices.HappyEating.Core;
    using Microsoft.Azure.WebJobs.Extensions.EventGrid;
    using Microsoft.Azure.EventGrid.Models;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.Domain.Campaign;
    using ServerlessMicroservices.HappyEating.Infrastructure;
    using System.Collections.Generic;
    using System.Linq;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.IntegrationEvent;

    public class EVGH_OrderCreatedEventExternalization2Campaign
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Campaign> campaigns;

        public EVGH_TransactionCreatedExternalization2Voyages(
            MongoClient mongoClient,
            ILogger<TransactionTracker> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            campaigns = database.GetCollection<Campaign>("Campaigns");
        }

        [FunctionName(nameof(EVGH_OrderCreatedEventExternalization2Campaign))]
        public async void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation($"EVGH_OrderCreatedEventExternalization2Campaign triggered....EventGridEvent" +
                            $"\n\tId:{ eventGridEvent.Id }" +
                            $"\n\tTopic:{ eventGridEvent.Topic }" +
                            $"\n\tSubject:{ eventGridEvent.Subject }" +
                            $"\n\tType:{ eventGridEvent.EventType }" +
                            $"\n\tData:{ eventGridEvent.Data }");

            OrderCreatedEvent orderCreatedEvent = null;
            Campaign existingCampaign = null;

            try
            {
                orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(eventGridEvent.Data.ToString());

                if (orderCreatedEvent == null)
                    throw new Exception("order is null!");

                log.LogInformation($"EVGH_OrderCreatedExternalization2Campaign transaction {orderCreatedEvent.OrderId}");

               
                if (orderCreatedEvent == null || string.IsNullOrEmpty(orderCreatedEvent.CampaignId))
                    throw new Exception("A order with a valid CampaignId must be attached to the order!!");

                existingCampaign = await campaigns.Find(v => v.Id == orderCreatedEvent.CampaignId).FirstOrDefaultAsync();

                existingCampaign.TryMapOrderCreatedEventToCampaign(orderCreatedEvent);

                var replacedItem = await existingCampaign.ReplaceOneAsync(t => t.Id == transaction.Id, existingCampaign);
                await RaiseCampaignOrderAcceptedEvent(orderCreatedEvent, existingCampaign, Constants.EVG_EVENT_CAMPAIGN_ORDER_ACCEPTED);
                
            }
            catch (Exception e)
            {
                var error = $"Updating Campaign failed: {e.Message}";
                logger.LogError(error);

                await RaiseCampaignOrderRejectedEvent(orderCreatedEvent, existingCampaign, error, Constants.EVG_EVENT_CAMPAIGN_ORDER_REJECTED);
            }
        }

        private static async Task RaiseCampaignOrderAcceptedEvent(OrderCreatedEvent orderCreatedEvent, Campaign campaign, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<CampaignOrderAcceptedEvent>(null,
                new CampaignOrderAcceptedEvent()
                {
                    OrderId = orderCreatedEvent.OrderId,
                    ActualOrderedUnit = orderCreatedEvent.NumberOfOrderedUnit,
                    AvailableUnit = campaign.NumberOfAvailableUnit
                },
                Constants.EVG_EVENT_CAMPAIGN_ORDER_ACCEPTED,
                subject,
                Settings.GetCampaignExternalizationsEventGridTopicUrl(),
                Settings.GetCampaignExternalizationsEventGridTopicApiKey());
        }

        private static async Task RaiseCampaignOrderRejectedEvent(OrderCreatedEvent orderCreatedEvent, Campaign campaign, string failedReason, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<CampaignOrderRejectedEvent>(null,
                new CampaignOrderRejectedEvent()
                {
                    OrderId = orderCreatedEvent.OrderId,
                    ActualOrderedUnit = orderCreatedEvent.NumberOfOrderedUnit,
                    AvailableUnit = campaign.NumberOfAvailableUnit,
                    FailReason = failedReason
                },
                Constants.EVG_EVENT_CAMPAIGN_ORDER_REJECTED,
                subject,
                Settings.GetCampaignExternalizationsEventGridTopicUrl(),
                Settings.GetCampaignExternalizationsEventGridTopicApiKey());
        }
    }
}
