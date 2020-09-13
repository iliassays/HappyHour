using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using HappyEating.Core;
using HappyEating.Campaign.Core;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HappyEating.Core
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddDaprClient(d =>
            //{
                //d.UseEndpoint("")
           // });

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            //var config = (IConfiguration) builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                MongoClient client = new MongoClient(config[Constants.MONGO_CONNECTION_STRING]);

                return client;
            });
        }
    }
}


