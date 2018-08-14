using System;
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

        #endregion

        #region construction

        public static IThumbnailOp New(ILocalCache<byte[]> cache)
        {
            return
                new ThumbnailOp(cache);
        }

        private ThumbnailOp(ILocalCache<byte[]> cache)
        {
            _log = TopicLogger.New("thumbnail-op");
            _cache = cache;
        }

        #endregion

        #region private

        private void handleError(ThumbnailRequest request, string key, Exception error)
        {
            _log.error(request.Srv.TrackingId, error, $"While waiting for '{key}' key");
        }

        private bool runStateMachine(string state, ThumbnailRequest request, byte[] image = null)
        {
            void onError(string key, Exception error) =>
                handleError(request, key, error);

            var thumbnailKey = request.Key;
            var downloadKey = request.Url;

            switch (state)
            {
                case null:
                case "":
                case "start":
                    var thumbnailItem = _cache.Get(thumbnailKey);

                    if (thumbnailItem.Value != null)
                        return runStateMachine("thumbnail-ready", request, thumbnailItem.Value);

                    _async.WhenReady(downloadKey,
                        bytes => runStateMachine("download-ready", request, bytes));

                    _async.WhenReady(thumbnailKey,
                        bytes => runStateMachine("thumbnail-ready", request, bytes));

                    if (thumbnailItem.State == CacheState.New)
                    {
                        Task<byte[]> downloadTask = null;
                        _async.Signal(downloadKey, downloadTask, onError);
                    }

                    return false;

                case "download-ready":
                    _cache.Put(downloadKey, image);

                    if (_async.RegisterSignal(thumbnailKey))
                    {
                        Task<byte[]> thumbnailTask = null;
                        _async.Signal(thumbnailKey, thumbnailTask, onError);
                    }

                    return false;

                case "thumbnail-ready":
                    _cache.Put(thumbnailKey, image);
                    request.Srv.EndWith(image);
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