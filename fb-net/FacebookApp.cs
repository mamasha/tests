using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace FbApi
{
    interface IFacebookApp
    {
        FbConfig Config { get; }
        object Connect();
        void ConnectionAccepted(string code);
        FbUser WaitForConnection();
        UpPhoto UploadPhoto(PhotoRqst photo, Stream fileStream);
        FbPost PublishPost(PostRqst post);
        void Disconnect();
    }

    class FacebookApp : IFacebookApp
    {
        #region members

        private readonly ILogger _log;
        private readonly string _sessionId;
        private readonly FbConfig _config;
        private readonly ManualResetEvent _connected;
        private readonly IGraphApi _api;
        private readonly IPhotoUploader _uploader;

        private string _token;
        private FbUser _me;

        #endregion

        #region construction

        public static IFacebookApp New(string sessionId, FbConfig config, IWebApi api, ILogger log)
        {
            return FacebookAppDTail.New(
                new FacebookApp(sessionId, config, api, log));
        }

        private FacebookApp(string sessionId, FbConfig config, IWebApi api, ILogger log)
        {
            _log = log;
            _sessionId = sessionId;
            _config = config;
            _connected = new ManualResetEvent(false);
            _api = GraphApi.New(config, api);
            _uploader = PhotoUploader.New();

            trace(config.ToJson());
        }

        #endregion

        #region private

        private void trace(string message)
        {
            var line = $"{_sessionId}: {message}";
            _log.Trace(line);
        }

        private string requirePageToken(string pageId)
        {
            var query =
                from page in _me.Pages
                where page.Id == pageId
                select page.Token;

            var token = query.FirstOrDefault();

            if (token.IsEmpty())
                throw new ApplicationException($"Page {pageId} is not found");

            return token;
        }

        #endregion

        #region interface

        FbConfig IFacebookApp.Config => _config;

        object IFacebookApp.Connect()
        {
            var connected = _connected.WaitOne(0);
            var status = connected ? "already connected" : "pending for facebook...";
            trace($"connection request ({status})");
            var config = new {
                status,
                appId = _config.AppId,
                apiVer = _config.ApiVer,
                redirectUrl = _config.RedirectUrl,
                perms = _config.RequiredPermissions
            };
            return config;
        }

        void IFacebookApp.ConnectionAccepted(string code)
        {
            _token = _api.Code2Token(code);
            trace($"user access token: '{_token}'");
            _me = _api.Me(_token);
            _connected.Set();
        }

        FbUser IFacebookApp.WaitForConnection()
        {
            var timeout = $"{_config.WaitSpan.TotalSeconds} seconds";
            var connected = _connected.WaitOne(_config.WaitSpan);
            if (!connected)
                throw new ApplicationException($"Session '{_sessionId}': Timeout ({timeout}) while waiting for facebook response");
            Trace.Assert(_me != null);
            trace(_me.ToJson());
            return _me;
        }

        UpPhoto IFacebookApp.UploadPhoto(PhotoRqst photo, Stream fileStream)
        {
            var uploadId = photo.uploadId;
            var token = requirePageToken(photo.pageId);

            var fbPhoto = _api.UploadPhoto(photo, token, fileStream);
            _uploader.PhotoIsReady(uploadId, fbPhoto);

            var reply = new UpPhoto {
                uploadId = uploadId,
                photoId = fbPhoto.Id,
                pageId = photo.pageId,
                length = photo.fileSize,
                fileName = photo.fileName,
                contentType = photo.fileType
            };

            return reply;
        }

        FbPost IFacebookApp.PublishPost(PostRqst post)
        {
            var token = requirePageToken(post.pageId);

            var photos = _uploader.WaitFor(post.uploadIds, _config.WaitSpan);
            var reply = _api.PublishPost(post, photos, token);

            return reply;
        }

        void IFacebookApp.Disconnect()
        {
        }

        #endregion
    }

    class FacebookAppDTail : IFacebookApp
    {
        private readonly IFacebookApp _;
        private readonly DetailsOnException d = new DetailsOnException();

        public static IFacebookApp New(IFacebookApp target) { return new FacebookAppDTail(target); }
        private FacebookAppDTail(IFacebookApp target) { _ = target; }

        FbConfig IFacebookApp.Config => _.Config;
        object IFacebookApp.Connect() => _.Connect();
        void IFacebookApp.ConnectionAccepted(string code) => _.ConnectionAccepted(code);
        FbUser IFacebookApp.WaitForConnection() => _.WaitForConnection();
        UpPhoto IFacebookApp.UploadPhoto(PhotoRqst photo, Stream fileStream) => d.tail(photo, () => _.UploadPhoto(photo, fileStream));
        FbPost IFacebookApp.PublishPost(PostRqst post) => d.tail(post, () => _.PublishPost(post));
        void IFacebookApp.Disconnect() => _.Disconnect();
    }
}