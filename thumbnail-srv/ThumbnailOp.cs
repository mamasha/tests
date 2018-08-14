using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ThumbnailSrv
{
    class ThumbnailRequest
    {
        public ISrvRequest Srv { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    interface IThumbnailOp
    {
        void Process(ThumbnailRequest request);
    }

    class ThumbnailOp : IThumbnailOp
    {
        #region members

        private readonly ITopicLogger _log;
        private readonly ILocalCache<byte[]> _cache;
        private readonly IAsyncFlow<byte[]> _async;
        private readonly WebClient _web;
        private readonly IImageUtilities _helpers;

        #endregion

        #region construction

        public static IThumbnailOp New(ILocalCache<byte[]> cache, IAsyncFlow<byte[]> async)
        {
            return
                new ThumbnailOp(cache, async);
        }

        private ThumbnailOp(ILocalCache<byte[]> cache, IAsyncFlow<byte[]> async)
        {
            _log = TopicLogger.New("thumbnail-op");
            _cache = cache;
            _async = async;
            _web = new WebClient();
            _helpers = ImageUtilities.New();
        }

        #endregion

        #region private

        private Task<byte[]> downloadImage(string url)
        {
            return Task.Run( 
                () => _web.DownloadData(url));
        }

        private Task<byte[]> resizeImage(byte[] image, int width, int height)
        {
            return Task.Run(
                () => _helpers.TextToImage(width, height, $"{width}x{height} thumbnail is ready"));
        }

        private bool runStateMachine(string state, ThumbnailRequest request, byte[] image = null, Exception error = null)
        {
            var trackingId = request.Srv.TrackingId;

            if (error != null)
            {
                _log.info(trackingId, () => $"Error on state '{state}'; {error.Message}");
                request.Srv.EndWith(error);
                return true;
            }

            var thumbnailKey = request.Key;
            var downloadKey = request.Url;

            switch (state)
            {
                case null:
                case "":
                case "start":
                    var (thumbnail, firstTouch) = _cache.Get(thumbnailKey);

                    var cacheHit = thumbnail != null;

                    _log.info(trackingId, () => $"{state}: firstTouch={firstTouch}, cacheHit={cacheHit}, downloadKey='{downloadKey}'");

                    if (cacheHit)
                        return runStateMachine("thumbnail-ready", request, thumbnail);

                    _async.WhenReady(downloadKey,
                        (bytes, ex) => runStateMachine("download-ready", request, bytes, ex));

                    if (firstTouch)
                        _async.WaitFor(downloadKey, downloadImage(request.Url));

                    return false;

                case "download-ready":
                    _cache.Put(downloadKey, image);

                    _async.WhenReady(thumbnailKey,
                        (bytes, ex) => runStateMachine("thumbnail-ready", request, bytes, ex));

                    var noResizeTaskIsThere = _async.TouchPoint(thumbnailKey);

                    _log.info(trackingId, () => $"{state}: noResizeTaskIsThere={noResizeTaskIsThere}, thumbnailKey='{thumbnailKey}'");

                    if (noResizeTaskIsThere)
                        _async.WaitFor(thumbnailKey, resizeImage(image, request.Width, request.Height));

                    return false;

                case "thumbnail-ready":
                    _cache.Put(thumbnailKey, image);
                    request.Srv.EndWith(image);

                    _log.info(trackingId, () => $"{state}: thumbnailKey='{thumbnailKey}'");

                    return true;

                default:
                    throw new Exception("Should not get here");
            }
        }

        #endregion

        #region interface

        void IThumbnailOp.Process(ThumbnailRequest request)
        {
            runStateMachine("start", request);
        }

        #endregion
    }
}