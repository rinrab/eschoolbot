using NUnit.Framework;

namespace ESchoolBot.Tests
{
    public class DatabaseTests
    {
        [Test]
        public void UpgradeTest()
        {
            using (var sb = new DatabaseSandbox())
            {
                DatabaseClient databaseClient = new DatabaseClient(null, sb.DatabaseAccessor);

                Assert.AreNotEqual(databaseClient.GetVersion(), 0);
            }
        }

        [Test]
        public void InsertUserTest()
        {
            using (var sb = new DatabaseSandbox())
            {
                DatabaseClient databaseClient = new DatabaseClient(null, sb.DatabaseAccessor);

                databaseClient.InsertUser(new DatabaseClient.User
                {
                    ChatId = 123,
                    Username = "amogus_username",
                    Password = "amogus_password",
                    SessionId = "amogus_session",
                    ProcessedDate = new DateTime(2020, 1, 4),
                    IsEnabled = true
                });

                CollectionAssert.AreEqual(
                    new DatabaseClient.User[]
                    {
                        new DatabaseClient.User
                        {
                            ChatId = 123,
                            Username = "amogus_username",
                            Password = "amogus_password",
                            SessionId = "amogus_session",
                            ProcessedDate = new DateTime(2020, 1, 4),
                            IsEnabled = true
                        }
                    },
                    databaseClient.ListUsers());
            }
        }

        [Test]
        public void InsertManyUsersTest()
        {
            using (var sb = new DatabaseSandbox())
            {
                DatabaseClient databaseClient = new DatabaseClient(null, sb.DatabaseAccessor);

                databaseClient.InsertUser(new DatabaseClient.User
                {
                    ChatId = 123,
                    Username = "amogus_username",
                    Password = "amogus_password",
                    SessionId = "amogus_session",
                    ProcessedDate = new DateTime(2020, 1, 4),
                    IsEnabled = true
                });

                databaseClient.InsertUser(new DatabaseClient.User
                {
                    ChatId = 124,
                    Username = "sus_username",
                    Password = "sus_password",
                    SessionId = "sus_session",
                    ProcessedDate = new DateTime(2020, 1, 6),
                    IsEnabled = true
                });

                CollectionAssert.AreEqual(
                    new DatabaseClient.User[]
                    {
                        new DatabaseClient.User
                        {
                            ChatId = 123,
                            Username = "amogus_username",
                            Password = "amogus_password",
                            SessionId = "amogus_session",
                            ProcessedDate = new DateTime(2020, 1, 4),
                            IsEnabled = true
                        },
                        new DatabaseClient.User
                        {
                            ChatId = 124,
                            Username = "sus_username",
                            Password = "sus_password",
                            SessionId = "sus_session",
                            ProcessedDate = new DateTime(2020, 1, 6),
                            IsEnabled = true
                        }
                    },
                    databaseClient.ListUsers());
            }
        }

        [Test]
        public void DisableUserTest()
        {
            using (var sb = new DatabaseSandbox())
            {
                DatabaseClient databaseClient = new DatabaseClient(null, sb.DatabaseAccessor);

                databaseClient.InsertUser(new DatabaseClient.User
                {
                    ChatId = 123,
                    Username = "amogus_username",
                    Password = "amogus_password",
                    SessionId = "amogus_session",
                    ProcessedDate = new DateTime(2020, 1, 4),
                    IsEnabled = true
                });

                databaseClient.DisableUser(123);

                CollectionAssert.AreEqual(
                    new DatabaseClient.User[]
                    {
                        new DatabaseClient.User
                        {
                            ChatId = 123,
                            Username = "amogus_username",
                            Password = "amogus_password",
                            SessionId = "amogus_session",
                            ProcessedDate = new DateTime(2020, 1, 4),
                            IsEnabled = false
                        }
                    },
                    databaseClient.ListUsers());
            }
        }

        [Test]
        public void UpdateProcessedDateTest()
        {
            using (var sb = new DatabaseSandbox())
            {
                DatabaseClient databaseClient = new DatabaseClient(null, sb.DatabaseAccessor);

                databaseClient.InsertUser(new DatabaseClient.User
                {
                    ChatId = 123,
                    Username = "amogus_username",
                    Password = "amogus_password",
                    SessionId = "amogus_session",
                    ProcessedDate = new DateTime(2020, 1, 4),
                    IsEnabled = true
                });

                databaseClient.UpdateProcessedDate(123, new DateTime(2020, 1, 6));

                CollectionAssert.AreEqual(
                    new DatabaseClient.User[]
                    {
                        new DatabaseClient.User
                        {
                            ChatId = 123,
                            Username = "amogus_username",
                            Password = "amogus_password",
                            SessionId = "amogus_session",
                            ProcessedDate = new DateTime(2020, 1, 6),
                            IsEnabled = true
                        }
                    },
                    databaseClient.ListUsers());
            }
        }
    }
}
