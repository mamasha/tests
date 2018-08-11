using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ThumbnailSrv
{
    interface IApi
    {
        void Thumbnail(AnyRequest my, string url, int width, int height);
        byte[] TextToImage(string text, int width, int height);
    }

    class Api : IApi
    {
        #region singleton

        public static IApi Instance { get; } = new Api();

        private Api()
        { }

        #endregion

        #region interface

        void IApi.Thumbnail(AnyRequest my, string url, int width, int height)
        {
            if (url != "http://thumbnail.src/test.jpg")
            {
                IApi self = this;
                my.Image = self.TextToImage($"404 - '{url}' is not found", width, height);
                my.NotifyCompletion();
                return;
            }

            var path = my.Http.Request.MapPath("assets/vladstudio_selfportrait2009.jpg");
            my.Image = File.ReadAllBytes(path);

            my.NotifyCompletion();
        }

        byte[] IApi.TextToImage(string text, int width, int height)
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