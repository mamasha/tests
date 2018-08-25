using System;
using System.Diagnostics;
using System.Drawing;

namespace ThumbnailSrv
{
    static class ResizeMath
    {
        public static Rectangle KeepRatio(decimal imgWidth, decimal imgHeight, decimal width, decimal height)
        {
            Rectangle rect(decimal x, decimal y, decimal dx, decimal dy) =>
                new Rectangle((int)x, (int)y, (int)dx, (int)dy);

            if (width >= imgWidth && height >= imgHeight)
            {
                var left = (width - imgWidth) / 2;
                var top = (height - imgHeight) / 2;
                return
                    rect(left, top, imgWidth, imgHeight);
            }

            var imgRatio = imgWidth / imgHeight;
            var ratio = width / height;

            if (ratio == imgRatio)
            {
                return
                    rect(0, 0, width, height);
            }

            if (ratio > imgRatio)     // width to be adjusted
            {
                var dx = height * imgRatio;
                Trace.Assert(width > dx);
                var left = (width - dx) / 2;
                return
                    rect(left, 0, dx, height);
            }

            if (ratio < imgRatio)     // height to be adjusted
            {
                var dy = width / imgRatio;
                Trace.Assert(height > dy);
                var top = (height - dy) / 2;
                return
                    rect(0, top, width, dy);
            }

            throw
                new Exception("Should not get here");
        }
    }
}