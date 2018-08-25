using System.Drawing;
using NUnit.Framework;

namespace ThumbnailSrv
{
    [TestFixture]
    class When_resizing
    {
        [Test]
        public void With_same_dimensions_They_are_kept()
        {
            Assert.IsTrue(ResizeMath
                .KeepRatio(300, 200, 300, 200)
                .SameAs(0, 0, 300, 200));
        }

        [Test]
        public void With_same_ratio_Destination_dimensions_are_kept()
        {
            Assert.IsTrue(ResizeMath
                .KeepRatio(300, 200, 150, 100)
                .SameAs(0, 0, 150, 100));
        }

        [Test]
        public void It_does_not_scale_up()
        {
            Assert.IsTrue(ResizeMath
                .KeepRatio(300, 200, 400, 300)
                .SameAs(50, 50, 300, 200));
        }

        [Test]
        public void With_different_ratio_Y_ration_is_adjusted()
        {
            Assert.IsTrue(ResizeMath
                .KeepRatio(300, 200, 75, 75)
                .SameAs(0, 12, 75, 50));
        }

        [Test]
        public void With_different_ratio_X_ration_is_adjusted()
        {
            Assert.IsTrue(ResizeMath
                .KeepRatio(300, 200, 280, 100)
                .SameAs(65, 0, 150, 100));
        }
    }

    static class Helpers
    {
        public static bool SameAs(this Rectangle rc, int x, int y, int dx, int dy)
        {
            return
                rc.Left == x && rc.Top == y && rc.Width == dx && rc.Height == dy;
        }
    }
}