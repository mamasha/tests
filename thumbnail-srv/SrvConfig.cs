namespace ThumbnailSrv
{
    class SrvConfig
    {
        public ThumbnailArgs thumbnail { get; set; }

        public class ThumbnailArgs
        {
            public int? defaultWidth { get; set; } = 200;
            public int? defaultHeight { get; set; } = 150;
        }
    }
}