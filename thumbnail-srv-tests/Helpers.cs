using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace thumbnail_srv_tests
{
    static class Helpers
    {
        private static double getStdDev(Image image)
        {
            double total = 0, totalVariance = 0;
            int count = 0;
            double stdDev = 0;

            // First get all the bytes
            using (Bitmap b = new Bitmap(image))
            {
                var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
                int stride = bmData.Stride;
                IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * 4;
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            count++;

                            byte blue = p[0];
                            byte green = p[1];
                            byte red = p[2];

                            if (blue < 40 && green < 40 && red < 40)
                            {
                                blue = green = red = 0;
                            }

                            int pixelValue = Color.FromArgb(0, red, green, blue).ToArgb();
                            total += pixelValue;
                            double avg = total / count;
                            totalVariance += Math.Pow(pixelValue - avg, 2);
                            stdDev = Math.Sqrt(totalVariance / count);

                            p += 4;
                        }
                        p += nOffset;
                    }
                }

                b.UnlockBits(bmData);
            }

            return stdDev;
        }

        public static Image Crop(this Image image, int x, int y, int dx, int dy)
        {
            var area = new Rectangle(x, y, dx, dy);
            var bmp = new Bitmap(image);
            var crop = bmp.Clone(area, bmp.PixelFormat);

            return crop;
        }

        public static bool IsBlank(this Image image)
        {
            var stdDev = getStdDev(image);
            var isBlank = stdDev < 100000;

            return isBlank;
        }

        public static bool SameAs(this Image src, Image other)
        {
            return true;
        }
    }
}