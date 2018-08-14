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
            var log = TopicLogger.New("thumbnail-op");

            _log = log;
            _cache = cache;
            _async = async;
            _helpers = ImageUtilities.New(log);
        }

        #endregion

        #region private

        private Task<byte[]> downloadImage(ThumbnailRequest request, string url)
        {
            var trackingId = request.Srv.TrackingId;

            byte[] download()
            {
                try
                {
                    _log.info(trackingId, () => $"Downloading '{url}'");

                    using (var web = new WebClient())
                    {
                        return
                            web.DownloadData(url);
                    }
                }
                catch (Exception ex)
                {
                    _log.error(trackingId, ex, $"Failed to download '{url}' ({ex.Message})");
                    return
                        _helpers.TextToImage(request.Width, request.Height, $"{ex.Message} (trackingId={trackingId})");
                }
            }

            return 
                Task.Run(() => download());
        }

        private Task<byte[]> resizeImage(ThumbnailRequest request, byte[] image)
        {
            var trackingId = request.Srv.TrackingId;
            int width = request.Width;
            int height = request.Height;

            byte[] resize()
            {
                try
                {
                    _log.info(trackingId, () => $"Resize and change format to {width}x{height} jpeg");
                    var bytes = _helpers.ResizeImage(trackingId, image, width, height);

                    if (bytes == null)
                        throw new ApplicationException($"'{request.Url}' probably is not an image");

                    return bytes;
                }
                catch (Exception ex)
                {
                    _log.error(trackingId, ex, $"Failed to resize an image ({ex.Message})");
                    return
                        _helpers.TextToImage(request.Width, request.Height, $"{ex.Message} (trackingId={trackingId})");
                }
            }

            return 
                Task.Run(() => resize());
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

                    _log.info(trackingId, () => $"{state} firstTouch={firstTouch}, cacheHit={cacheHit}");

                    if (cacheHit)
                        return runStateMachine("thumbnail-ready", request, thumbnail);

                    _async.WhenReady(downloadKey,
                        (bytes, ex) => runStateMachine("download-ready", request, bytes, ex));

                    if (firstTouch)
                        _async.WaitFor(downloadKey, downloadImage(request, request.Url));

                    return false;

                case "download-ready":
                    _cache.Put(downloadKey, image);

                    _async.WhenReady(thumbnailKey,
                        (bytes, ex) => runStateMachine("thumbnail-ready", request, bytes, ex));

                    var noResizeTaskIsThere = _async.TouchPoint(thumbnailKey);

                    _log.info(trackingId, () => $"{state} noResizeTaskIsThere={noResizeTaskIsThere}");

                    if (noResizeTaskIsThere)
                        _async.WaitFor(thumbnailKey, resizeImage(request, image));

                    return false;

                case "thumbnail-ready":
                    _cache.Put(thumbnailKey, image);
                    request.Srv.EndWith(image);

                    _log.info(trackingId, () => $"{state}");

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