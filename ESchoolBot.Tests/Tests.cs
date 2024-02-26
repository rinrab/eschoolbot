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
    }
}