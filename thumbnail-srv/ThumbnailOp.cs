using System;
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
        void AsyncFlow(ThumbnailRequest request);
    }

    class ThumbnailOp : IThumbnailOp
    {
        #region members

        private readonly ITopicLogger _log;
        private readonly IImageCache _cache;

        #endregion

        #region construction

        public static IThumbnailOp New(IImageCache cache)
        {
            return
                new ThumbnailOp(cache);
        }

        private ThumbnailOp(IImageCache cache)
        {
            _log = TopicLogger.New("thumbnail-op");
            _cache = cache;
        }

        #endregion

        #region private

/*
        private void asyncFlow(string state, ThumbnailRequest request, byte[] image = null)
        {
            var trackingId = request.Srv.TrackingId;

            switch (state)
            {
                case "start":
                    {
                        var item = _local.Get(request.Key);

                        if (item.State == CacheState.Ready)
                        {
                            thumbnailFlow("image-ready", request, item.Image);
                            return;
                        }

                        _async.WhenReady(request.Key,
                            bytes => thumbnailFlow("local-cache-ready", request, bytes));

                        if (item.State == CacheState.New)
                        {
                            var task = Task.Run(() => { });
                            _async.Set(request.Key, task);
                        }

                        return;
                    }

                case "local-cache-ready":
                    {
                        _async.Resolve(request.Key, image);
                        var bytes = _helpers.Rescale(image, request.Width, request.Height);
                        thumbnailFlow("image-ready", request, bytes);
                        return;
                    }

                case "image-ready":
                    _local.Put(request.Key, image);
                    request.Srv.EndWith(image);
                    return;

                default:
                    _log.error(trackingId, null, $"Unexpected state '{state ?? "n/a"}'");
                    return;
            }
        }
*/

        private void asyncFlow2(string state, ThumbnailRequest request, byte[] image = null)
        {
            void next(string toState, Action action)
            {
                asyncFlow2(toState, request);
            }

            switch (state)
            {
                case "start":
                    next("cache-new", () => _cache.Get(""));
                    break;

                case "cache-new":
                    next("cache-pending", () => _cache.Get(""));
                    break;

                case "cache-pending":
                    next("cache-ready", () => _cache.Get(""));
                    break;

                case "cache-ready":
                    next("resize-ready", () => _cache.Get(""));
                    break;

                case "resize-ready":
                    break;
            }
        }

        #endregion

        #region interface

        void IThumbnailOp.AsyncFlow(ThumbnailRequest request)
        {
        }

        #endregion
    }
}