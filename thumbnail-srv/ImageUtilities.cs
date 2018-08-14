using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Hosting;

namespace ThumbnailSrv
{
    interface IImageUtilities
    {
        byte[] TextToImage(int width, int height, string text);
        byte[] ResizeImage(byte[] src, int width, int height);
    }

    class ImageUtilities : IImageUtilities
    {
        #region construction

        public static IImageUtilities New()
        {
            return
                new ImageUtilities();
        }

        private ImageUtilities()
        { }

        #endregion

        #region interface

        byte[] IImageUtilities.TextToImage(int width, int height, string text)
        {
            var img = new Bitmap(width, height);
            var drawing = Graphics.FromImage(img);
            var font = new Font("Ariel", 12);
            var brush = new SolidBrush(Color.White);
            var rect = new RectangleF(0, 0, width, height);

            drawing.Clear(Color.Black);
            drawing.DrawString(text, font, brush, rect);
            drawing.Save();

            var memory = new MemoryStream();
            img.Save(memory, ImageFormat.Jpeg);
            var bytes = memory.ToArray();

            brush.Dispose();
            font.Dispose();
            drawing.Dispose();
            img.Dispose();

            return bytes;
        }

        private static Bitmap resizeImageToBitmap(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        byte[] IImageUtilities.ResizeImage(byte[] src, int width, int height)
        {
            using (var srcMem = new MemoryStream(src))
            using (var srcImg = Image.FromStream(srcMem))
            {
                var sameSize =
                    srcImg.Width == width && srcImg.Height == height;

                var noResizeIsNeeded =
                    sameSize && ImageFormat.Jpeg.Equals(srcImg.RawFormat);

                if (noResizeIsNeeded)
                    return src;

                var dstImg = sameSize ? srcImg :
                    resizeImageToBitmap(srcImg, width, height);

                using (var dstMem = new MemoryStream())
                {
                    dstImg.Save(dstMem, ImageFormat.Jpeg);

                    return 
                        dstMem.ToArray();
                }
            }
        }

        #endregion
    }
}