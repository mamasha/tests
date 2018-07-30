using System;
using System.IO;
using System.Linq;
using System.Web;

namespace FbApi
{
    class Config
    {
        public string LogPath = "c:/tmp";
        public string LogFile = "fb-api.log";
        public FbConfig FbConfig { get; set; } = new FbConfig {
            ApiBaseUrl = "https://graph.facebook.com",
            ApiVer = "v3.0",
            AppId = "1959695444359871",
            AppSecret = "9cfa4fa2d2971e9cdc0c312e91908eae",
            RedirectUrl = "https://localhost/fb-accepted",
            RequiredPermissions = "manage_pages,pages_show_list,publish_pages,pages_messaging,pages_manage_cta",
            WaitSpan = 60.Seconds(),
        };
    }

    interface IAppController
    {
        Config Config { get; }
        ILogger Logger { get; }
        object Init(string sessionId);
        object Connect(string sessionId);
        object WaitForConnection(string sessionId);
        string ConnectionAccepted(string sessionId, string code);
        object UploadPhoto(string sessionId, PhotoRqst photo, HttpContext http);
        object PublishPost(string sessionId, PostRqst post);
        object Disconnect(string sessionId);
    }

    class AppController : IAppController
    {
        #region members
        private static readonly DetailsOnException d = new DetailsOnException();

        private readonly ILogger _log;
        private readonly Config _config;
        private readonly IAppRepo _repo;
        private readonly IWebApi _api;

        #endregion

        #region singleton

        private AppController(Config config)
        {
            _log = Logger.New(Path.Combine(config.LogPath, config.LogFile));
            _config = config;
            _repo = SyncRepo.New(AppRepo.New());
            _api = WebApi.New(_log);
        }

        public static IAppController Instance { get; } = new AppController(new Config());

        #endregion

        #region private

        private IFacebookApp getApp(string sessionId)
        {
            var app = _repo.GetApp(sessionId);

            if (app == null)
                throw new ApplicationException($"Sessions '{sessionId}' is not initialized");

            return app;
        }

        #endregion

        #region interface

        ILogger IAppController.Logger => _log;
        Config IAppController.Config => _config;

        object IAppController.Init(string sessionId)
        {
            var app = FacebookApp.New(sessionId, _config.FbConfig, _api, _log);

            if (!_repo.AddApp(sessionId, app))
                throw new ApplicationException($"Sessions '{sessionId}' is already initialized");

            return new {
                status = "session is initialized"
            };
        }

        object IAppController.Connect(string sessionId)
        {
            var app = getApp(sessionId);
            var config = app.Connect();
            return config;
        }

        object IAppController.WaitForConnection(string sessionId)
        {
            var app = getApp(sessionId);
            var me = app.WaitForConnection();

            var pages =
                from page in me.Pages
                select new {
                    name = page.Name,
                    id = page.Id
                };

            return new {
                userName = me.UserName,
                userId = me.UserId,
                pages = pages.ToArray()
            };
        }

        string IAppController.ConnectionAccepted(string sessionId, string code)
        {
            var app = getApp(sessionId);
            app.ConnectionAccepted(code);
            return @"
                <body>
                <script>
                    window.close()
                </script>
                </body>";
        }

        [Serializable]
        class FileInfo
        {
            public string fileName { get; set; }
            public string contentType { get; set; }
            public int contentLength { get; set; }
        }

        object IAppController.UploadPhoto(string sessionId, PhotoRqst photo, HttpContext http)
        {
            var request = http.Request;

            var files = request.Files;
            var count = files.Count;

            if (count == 0)
                throw new ApplicationException("No files in upload request");

            if (count > 1)
                throw new ApplicationException($"More than one ({count}) files in upload request");

            var file = files.Get(0);

            var info = new FileInfo {
                fileName = file.FileName,
                contentType = file.ContentType,
                contentLength = file.ContentLength
            };

            var app = getApp(sessionId);

            using (var fileStream = file.InputStream)
            {
                var response = d.tail(info,
                    () => app.UploadPhoto(photo, fileStream));

                return response;
            }
        }

        object IAppController.PublishPost(string sessionId, PostRqst post)
        {
            var app = getApp(sessionId);
            var response = app.PublishPost(post);

            return response;
        }

        object IAppController.Disconnect(string sessionId)
        {
            var app = getApp(sessionId);
            app.Disconnect();
            return "{err: 'Disconnect is not implemented yet'}";
        }

        #endregion
    }
}