using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ThumbnailSrv
{
    interface IImageUtilities
    {
        byte[] TextToImage(int width, int height, string text);
        byte[] ResizeImage(string trackingId, byte[] src, int width, int height);
    }

    class ImageUtilities : IImageUtilities
    {
        #region members

        private readonly ITopicLogger _log;

        #endregion

        #region construction

        public static IImageUtilities New(ITopicLogger log)
        {
            return
                new ImageUtilities(log);
        }

        private ImageUtilities(ITopicLogger log)
        {
            _log = log;
        }

        #endregion

        #region private

        private void fillMargins(Graphics graphics, int dx, int dy, Rectangle core, Brush brush)
        {
            var sameDims =
                core.Width == dx && core.Height == dy;

            if (sameDims)
                return;

            var x1 = core.Left;
            var x2 = x1 + core.Width;
            var y1 = core.Top;
            var y2 = y1 + core.Height;

            var margins = new[] {
                new Rectangle(0, 0, dx, y1),
                new Rectangle(0, y2, dx, dy - y2),
                new Rectangle(0, y1, x1, y2 - y1),
                new Rectangle(x2, y1, dx - x2, y2 - y1)
            };

            graphics.FillRectangles(brush, margins);
        }

        private Bitmap resizeImageToBitmap(string trackingId, Image image, int width, int height)
        {
            var innerRect = ResizeMath.KeepRatio(image.Width, image.Height, width, height);

            _log.info(trackingId, () => $"resize  {image.Width}x{image.Height} --> {width}x{height}  =  [{innerRect.Top} {innerRect.Left} {innerRect.Width}x{innerRect.Height}]");

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
                    graphics.DrawImage(image, innerRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }

                fillMargins(graphics, width, height, innerRect, Brushes.Black);
            }

            return destImage;
        }

        private Image mem2image(string trackingId, MemoryStream mem)
        {
            try
            {
                return
                    Image.FromStream(mem);
            }
            catch (Exception ex)
            {
                _log.error(trackingId, ex, $"Failed to create image from data; {ex.Message}");
                return null;
            }
        }

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

        byte[] IImageUtilities.ResizeImage(string trackingId, byte[] src, int width, int height)
        {
            using (var srcMem = new MemoryStream(src))
            using (var srcImg = mem2image(trackingId, srcMem))
            {
                if (srcImg == null)
                    return null;

                var sameSize =
                    srcImg.Width == width && srcImg.Height == height;

                var noResizeIsNeeded =
                    sameSize && ImageFormat.Jpeg.Equals(srcImg.RawFormat);

                if (noResizeIsNeeded)
                {
                    _log.info(trackingId, () => "same size, same format (resize is skipped)");
                    return src;
                }

                var dstImg = sameSize ? srcImg :
                    resizeImageToBitmap(trackingId, srcImg, width, height);

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