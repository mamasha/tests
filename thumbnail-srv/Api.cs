using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ThumbnailSrv
{
    interface IApi
    {
        void SetConfig(SrvConfig srvConfig);
        void Thumbnail(ISrvRequest request);
    }

    class Api : IApi
    {
        #region config

        public class Config
        {
            public ThumbnailArgs thumbnail { get; set; } = new ThumbnailArgs();

            public class ThumbnailArgs
            {
                public int defaultWidth { get; set; } = 200;
                public int defaultHeight { get; set; } = 150;
            }
        }

        #endregion

        #region members

        private readonly ITopicLogger _log;
        private readonly IThumbnailOp _thumbnail;

        private Config _config;

        #endregion

        #region singleton

        public static IApi Instance { get; } = new Api(new Config());

        private Api(Config config)
        {
            var cache = LocalCache<byte[]>.New();
            var async = AsyncFlow<byte[]>.New();

            _log = TopicLogger.New("api");
            _thumbnail = ThumbnailOp.New(cache, async);
            _config = config;
        }

        #endregion

        #region private

        #endregion

        #region interface

        void IApi.SetConfig(SrvConfig srvConfig)
        {
            var config = new Config {
                thumbnail = new Config.ThumbnailArgs {
                    defaultWidth = srvConfig.thumbnail?.defaultWidth ?? _config.thumbnail.defaultWidth,
                    defaultHeight = srvConfig.thumbnail?.defaultHeight ?? _config.thumbnail.defaultHeight
                }
            };

            _config = config;
        }

        void IApi.Thumbnail(ISrvRequest request)
        {
            var config = _config;
            var http = request.Http.Request;
            var trackingId = request.TrackingId;

            var url = http.Require("url");
            var width = http.OptionalInt("width", config.thumbnail.defaultWidth);
            var height = http.OptionalInt("height", config.thumbnail.defaultHeight);

            _log.info(trackingId, () => $"thumbnail request [{width} x {height}] for '{url}'");

            var key = $"{url}_{width}x{height}";

            _thumbnail.Process(new ThumbnailRequest {
                Srv = request,
                Key = key,
                Url = url,
                Width = width,
                Height = height
            });

/*
            if (url != "http://thumbnail.src/test.jpg")
            {
                var errMsg = $"{trackingId} - '{url}' is not found";
                var bytes = _helpers.TextToImage(width, height, errMsg);
                _log.error(trackingId, errMsg);
                request.EndWith(bytes);
                return;
            }

            var path = http.MapPath("assets/demon.jpg");
            var image = File.ReadAllBytes(path);

            request.EndWith(image);
*/
        }

        #endregion
    }
}