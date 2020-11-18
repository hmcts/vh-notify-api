using System.Net.Http;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Notify.API;
using NotifyApi.Common.Configuration;
using NotifyApi.Common.Security;
using NotifyApi.DAL;
using NotifyApi.IntegrationTests.Contexts;
using NotifyApi.IntegrationTests.Helper;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;

namespace NotifyApi.IntegrationTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(IntTestContext context)
        {
            _configRoot = ConfigurationManager.BuildConfig("4E35D845-27E7-4A19-BE78-CDA896BF907D");
            context.Config = new Config();
            context.Tokens = new NotifyApiTokens();
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public void RegisterSecrets(IntTestContext context)
        {
            var azureOptions = RegisterAzureSecrets(context);
            RegisterDefaultData(context);
            RegisterHearingServices(context);
            RegisterDatabaseSettings(context);
            RegisterServer(context);
            RegisterApiSettings(context);
            GenerateBearerTokens(context, azureOptions);
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets(IntTestContext context)
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            context.Config.AzureAdConfiguration = azureOptions.Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.AzureAdConfiguration);
            return azureOptions;
        }

        private static void RegisterDefaultData(IntTestContext context)
        {
            // Method intentionally left empty.
        }

        private void RegisterHearingServices(IntTestContext context)
        {
            context.Config.VhServices = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.VhServices);
        }

        private void RegisterDatabaseSettings(IntTestContext context)
        {
            context.Config.DbConnection = Options.Create(_configRoot.GetSection("ConnectionStrings").Get<ConnectionStringsConfig>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.DbConnection);
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<NotifyApiDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(context.Config.DbConnection.VhNotifyApi);
            context.NotifyBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            context.TestDataManager = new TestDataManager(context.Config.VhServices, context.NotifyBookingsDbContextOptions);
        }

        private static void RegisterServer(IntTestContext context)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                    .UseKestrel(c => c.AddServerHeader = false)
                    .UseEnvironment("Development")
                    .UseStartup<Startup>();
            context.Server = new TestServer(webHostBuilder);
        }

        private static void RegisterApiSettings(IntTestContext context)
        {
            context.Response = new HttpResponseMessage(); 
        }

        private static void GenerateBearerTokens(IntTestContext context, IOptions<AzureAdConfiguration> azureOptions)
        {
            context.Tokens.NotifyApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                context.Config.VhServices.VhNotifyApiResourceId);
            context.Tokens.NotifyApiBearerToken.Should().NotBeNullOrEmpty();

            Zap.SetAuthToken(context.Tokens.NotifyApiBearerToken);
        }
    }
}

