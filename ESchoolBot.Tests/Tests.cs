using NUnit.Framework;
using System.Diagnostics;

namespace ESchoolBot.Tests
{
    public class Tests
    {
        [Test]
        public async Task LoginTest()
        {
            if (Debugger.IsAttached)
            {
                ESchoolClient client = new ESchoolClient(new HttpClient());
                await client.LoginAsync("<login>", "<password>", default);
            }
        }

        [Test]
        public void DatabaseTests()
        {
            try
            {
                File.Delete("test.db");
            }
            catch
            {
            }

            IDatabaseAccessor databaseAccessor = new DatabaseAccessor("Data Source=test.db");

            // Create table and upgrade
            IDatabaseClient databaseClient = new DatabaseClient(databaseAccessor);

            // Add user
            databaseClient.InsertUser(new DatabaseClient.User
            {
                ChatId = 123,
                Username = "amogus_username",
                Password = "amogus_password",
                SessionId = "amogus_session",
                ProcessedDate = new DateTime(2020, 1, 4),
                IsEnabled = true
            });

            // Upgrade database again
            databaseClient = new DatabaseClient(databaseAccessor);

            // Get users
            var list = databaseClient.ListUsers();
            Assert.AreEqual(123, list[0].ChatId);
            Assert.AreEqual(true, list[0].IsEnabled);

            databaseClient.DisableUser(123);

            Assert.AreEqual(false, databaseClient.ListUsers()[0].IsEnabled);
        }
    }
}