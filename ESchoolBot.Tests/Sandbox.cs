namespace ESchoolBot.Tests
{
    public class Sandbox : IDisposable
    {
        public string RootPath { get; }

        public Sandbox()
        {
            RootPath = Path.Combine(Path.GetTempPath(), "eschool_tests");

            Directory.CreateDirectory(RootPath);

            Console.WriteLine("RootPath: {0}", RootPath);
        }

        public void Dispose()
        {
        }
    }
}
