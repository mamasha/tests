using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        private readonly IImageUtilities _helpers;
        private readonly ITopicLogger _log;

        private Config _config;

        #endregion

        #region singleton

        public static IApi Instance { get; } = new Api(new Config());

        private Api(Config config)
        {
            _helpers = ImageUtilities.New();
            _log = TopicLogger.New("api");
            _config = config;
        }

        #endregion

        #region private

        private void log(ISrvRequest request, Func<string> getMsg, Func<object> getDetails = null)
        {
            _log.info(request.TrackingId, getMsg, getDetails);
        }

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

            var url = http.Require("url");
            var width = http.OptionalInt("width", config.thumbnail.defaultWidth);
            var height = http.OptionalInt("height", config.thumbnail.defaultHeight);

            _log.info(request.TrackingId, () => $"thumbnail request for '{url}' width={width} height={height}");

            if (url != "http://thumbnail.src/test.jpg")
            {
                var bytes = _helpers.TextToImage($"404 - '{url}' is not found", width, height);
                request.EndWith(bytes);
                return;
            }

            var path = http.MapPath("assets/demon.jpg");
            var image = File.ReadAllBytes(path);

            request.EndWith(image);
        }

        #endregion
    }
}