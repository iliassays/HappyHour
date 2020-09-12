

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

    public class GetRunningCampaignsForUser
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger logger;
        private readonly IConfiguration config;

        private readonly IMongoCollection<Campaign> campaigns;

        public GetRunningCampaignsForUser(
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

        [FunctionName(nameof(GetRunningCampaignsForUser))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "campaigns/{userId}")] HttpRequest req,
            string userId)
        {
            logger.LogInformation("GetRunningCampaignsForUser triggered...");

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
