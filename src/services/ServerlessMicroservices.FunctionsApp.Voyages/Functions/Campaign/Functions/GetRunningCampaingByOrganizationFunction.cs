

namespace ServerlessMicroservices.FunctionsApp.HappyEating.Function.Canpaign
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using ServerlessMicroservices.HappyEating.Core;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.Domain.Campaign;

    public class GetRunningCampaignsByOrganization
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Campaign> campaigns;

        public GetRunningCampaignsByOrganization(
            MongoClient mongoClient,
            ILogger<GetRunningCampaingByOrganization> logger,
            IConfiguration config)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
            this.config = config;

            var database = this.mongoClient.GetDatabase(config[Constants.DATABASE_NAME]);
            campaigns = database.GetCollection<Campaign>(config[Constants.COLLECTION_NAME]);
        }

        [FunctionName(nameof(GetRunningCampaignsByOrganization))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "campaigns/{organizerId}")] HttpRequest req,
            string organizerId)
        {
            logger.LogInformation("GetRunningCampaignsByOrganization triggered...");

            try
            {
                //await Utilities.ValidateToken(req);
                var result = await campaigns.Find(campaign => campaign.OrganizerId == id && campaign.CampaignStatus == CampaignStatus.Running).ToListAsync();
                
                return new OkObjectResult(result);
            }
            catch(Exception e)
            {
                var error = $"GetRunningCampaignsByOrganization failed: {e.Message}";
                logger.LogError(error);

                return new BadRequestObjectResult(error);
            }
        }
    }
}
