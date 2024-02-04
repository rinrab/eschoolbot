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
                Client client = new Client(new HttpClient());
                await client.LoginAsync("<login>", "<password>", default);
            }
        }
    }
}