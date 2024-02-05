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
            databaseClient.InsertUser(123, "amogus_username", "amogus_password", "amogus_session", 456, 789);

            // Upgrade database again
            databaseClient = new DatabaseClient(databaseAccessor);

            // Get users
            var list = databaseClient.ListUsers();
            Assert.AreEqual(123, list[0].ChatId);

            databaseClient.UpdateProcessedDiaries(123, 10);
            Assert.AreEqual(10, databaseClient.ListUsers()[0].ProcessedDiaries);
        }
    }
}