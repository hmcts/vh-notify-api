using NotificationApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotificationApi.IntegrationTests.Helper;
using NUnit.Framework;

namespace NotificationApi.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        private string _databaseConnectionString;
        protected DbContextOptions<NotificationsApiDbContext> NotifyBookingsDbContextOptions;
        protected TestDataManager TestDataManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();
            _databaseConnectionString = configRoot.GetConnectionString("VhNotificationsApi");

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<NotificationsApiDbContext>();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            NotifyBookingsDbContextOptions = dbContextOptionsBuilder.Options;

            var context = new NotificationsApiDbContext(NotifyBookingsDbContextOptions);
            context.Database.Migrate();
            
            TestDataManager = new TestDataManager(NotifyBookingsDbContextOptions);
        }
    }
}
