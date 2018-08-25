using NUnitLite;

namespace thumbnail_srv_tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IntegrationlTests.Config = new TestConfig {
                BaseUrl = "127.0.0.1"
            };

            return 
                new AutoRun().Execute(args);
        }
    }
}