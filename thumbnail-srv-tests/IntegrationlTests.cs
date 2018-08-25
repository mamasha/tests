using System.Drawing;
using System.IO;
using System.Net;
using NUnit.Framework;

namespace thumbnail_srv_tests
{
    class TestConfig
    {
        public string BaseUrl { get; set; } = "http://127.0.0.1:8090";
        public string RemoteBase { get; set; } = "https://res.cloudinary.com/demo/image/upload";
        public string RemoteImg { get; set; } = "";
    }

    [TestFixture]
    class IntegrationlTests
    {
        public static TestConfig Config = new TestConfig();

        private static Image downloadImage(string url)
        {
            using (var http = new WebClient())
            {
                var bytes = http.DownloadData(url);
                var mem = new MemoryStream(bytes);

                return
                    Image.FromStream(mem);
            }
        }

        private static Image getThumbnail(int width, int height, string remote)
        {
            var api = $"{Config.BaseUrl}/thumbnail";
            var src = $"{Config.RemoteBase}/{remote}";

            var url = $"{api}?width={width}&height={height}&url={src}";

            return
                downloadImage(url);
        }

        private static Image getSource(string remote)
        {
            var src = $"{Config.RemoteBase}/{remote}";

            return
                downloadImage(src);
        }

        private static void assertSameImages(Image image, Image src)
        {
            Assert.That(image.Width, Is.EqualTo(src.Width));
            Assert.That(image.Height, Is.EqualTo(src.Height));
        }

        [Test]
        public void Get_remote_image()
        {
            var image = getThumbnail(300, 200, "w_300,h_200/sample.png");
            var src = getSource("w_300,h_200/sample.png");

            assertSameImages(image, src);
        }
    }
}