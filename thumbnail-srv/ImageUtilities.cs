using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ThumbnailSrv
{
    interface IImageUtilities
    {
        byte[] TextToImage(int width, int height, string text);
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

        #endregion
    }
}