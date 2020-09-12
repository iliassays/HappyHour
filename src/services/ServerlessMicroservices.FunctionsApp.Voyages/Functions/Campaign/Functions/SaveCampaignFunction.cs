

namespace ServerlessMicroservices.FunctionsApp.HappyEating.Function
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using ServerlessMicroservices.Voyages.Core;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.Domain.Campaign;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.VoyageMaker.Command;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.IntegrationEvent;

    public class SaveCampaign
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Campaign> campaigns;

        public SaveCampaign(
            MongoClient mongoClient,
            ILogger<Campaign> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            campaigns = database.GetCollection<Campaign>("Campaigns");
        }

        [FunctionName(nameof(SaveVoyageMaker))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "campaigns")] HttpRequest req)
        {
            logger.LogInformation("SaveCampaign triggered...");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var command = JsonConvert.DeserializeObject<SaveCampaignCommand>(requestBody);

                if (command == null || string.IsNullOrEmpty(command.OrganizationName) || string.IsNullOrEmpty(command.ProductId))
                    throw new Exception("A campaign with a valid data must be attached!!");

                var existingCampaign = await campaigns.Find(v => v.Id == command.Id).FirstOrDefaultAsync();

                var campaign = existingCampaign != null ? existingCampaign : new Campaign(command.Id, command.ProductId, command.Title, command.Description, command.NumberOfUnit, command.OrganizationName, command.OrganizationPhone);

                if (existingCampaign == null)
                {
                    campaigns.InsertOne(campaign);
                    await RaiseCampaignCreatedEvent(campaign, Constants.EVG_EVENT_CAMPAIGN_CREATED);
                }
                else
                {
                    existingCampaign.UpdateCampaign(command.Title, command.Description, command.NumberOfUnit);

                    var replacedItem = campaigns.ReplaceOne(v => v.Id == voyageMaker.Id, existingCampaign);
                    await RaiseCampaignCreatedEvent(replacedItem, Constants.EVG_EVENT_CAMPAIGN_UPDATED);
                }

                return new OkObjectResult(campaign);
            }
            catch(Exception e)
            {
                var error = $"SaveCampaign failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }

        private static async Task RaiseCampaignCreatedEvent(Campaign campaign, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<CampaignCreatedEvent>(null,
                new CampaignCreatedEvent()
                {
                    
                },
                Constants.EVG_EVENT_CAMPAIGN_CREATED,
                subject,
                Settings.GetCampaignExternalizationsEventGridTopicUrl(),
                Settings.GetCampaignExternalizationsEventGridTopicApiKey());
        }

        private static async Task RaiseCampaignUpdatedEvent(Campaign campaign, string subject)
        {
            await EventPublisher.TriggerEventGridTopic<CampaignUpdatedEvent>(null,
                new CampaignUpdatedEvent()
                {

                },
                Constants.EVG_EVENT_CAMPAIGN_UPDATED,
                subject,
                Settings.GetCampaignExternalizationsEventGridTopicUrl(),
                Settings.GetCampaignExternalizationsEventGridTopicApiKey());
        }
    }
}
