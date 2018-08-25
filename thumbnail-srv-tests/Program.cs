using NUnitLite;

namespace thumbnail_srv_tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IntegrationlTests.Config = new TestConfig {
                BaseUrl = "https://thumbnail-srv.azurewebsites.net"
            };

            return 
                new AutoRun().Execute(args);
        }
    }
}