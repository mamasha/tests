using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
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

        private static void assertSameImages(Image src, Image other)
        {
            Assert.That(other.Width, Is.EqualTo(src.Width));
            Assert.That(other.Height, Is.EqualTo(src.Height));

            Assert.True(other.SameAs(src));
        }

        [Test]
        public void No_resize()
        {
            var thumbnail = getThumbnail(300, 200, "w_300,h_200/sample.jpeg");
            var src = getSource("w_300,h_200/sample.jpeg");

            assertSameImages(src, thumbnail);
        }

        [Test]
        public void Same_ratio()
        {
            var thumbnail = getThumbnail(150, 100, "w_300,h_200/sample.jpeg");
            var src = getSource("w_150,h_100/sample.jpeg");

            assertSameImages(src, thumbnail);
        }

        [Test]
        public void No_scale_up()
        {
            var thumbnail = getThumbnail(400, 300, "w_300,h_200/sample.jpeg");
            var src = getSource("w_300,h_200/sample.jpeg");

            assertSameImages(src, thumbnail.Crop(50, 50, 300, 200));

            Assert.True(thumbnail.Crop(0, 0, 400, 50).IsBlank());
            Assert.True(thumbnail.Crop(0, 250, 400, 50).IsBlank());

            Assert.True(thumbnail.Crop(0, 0, 50, 300).IsBlank());
            Assert.True(thumbnail.Crop(350, 0, 50, 300).IsBlank());
        }

        [Test]
        public void Resize_different_ratio_adjust_Y_ratio()
        {
            var thumbnail = getThumbnail(75, 75, "w_300,h_200/sample.jpeg");
            var src = getSource("w_75,h_50/sample.jpeg");

            assertSameImages(thumbnail.Crop(0, 12, 75, 50), src);

            Assert.True(thumbnail.Crop(0, 0, 75, 12).IsBlank());
            Assert.True(thumbnail.Crop(0, 62, 75, 13).IsBlank());
        }

        [Test]
        public void Resize_different_ratio_adjust_X_ratio()
        {
            var thumbnail = getThumbnail(280, 100, "w_300,h_200/sample.jpeg");
            var src = getSource("w_150,h_100/sample.jpeg");

            assertSameImages(thumbnail.Crop(65, 0, 150, 100), src);

            Assert.True(thumbnail.Crop(0, 0, 65, 100).IsBlank());
            Assert.True(thumbnail.Crop(215, 0, 65, 100).IsBlank());
        }
    }
}