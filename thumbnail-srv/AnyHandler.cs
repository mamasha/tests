using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace ThumbnailSrv
{
    interface IAnyHandler
    {
        void WriteResponse(AnyResponse data);
        void NotifyCompletion();
    }

    public class AnyHandler : IHttpAsyncHandler, IAsyncResult, IAnyHandler
    {
        #region members

        private ILogger _log;
        private AsyncCallback _notifyCompletion;
        private bool _isCompleted;
        private ISrvRequest _request;

        #endregion

        #region private

        private void logStart(ISrvRequest request)
        {
        }

        private void logEnd(ISrvRequest request)
        {
        }

        private void writeResponse(AnyResponse data)
        {
            var userError = data.Error is ApplicationException;
            var responseIsData = data.Json != null;
            var responseIsImage = data.Image != null;
            var trackingId = _request.TrackingId;

            var response = _request.Http.Response;

            if (data.Error != null)
                _log.info(_request.TrackingId, "anyhandler", data.Error);

            if (userError)
            {
                response.ContentType = "application/json";
                response.StatusCode = 400;
                var d = new {
                    Error = $"{data.Error.Message} (trackingId='{trackingId}')"
                };
                response.Write(d.ToJson());
                return;
            }

            if (responseIsData)
            {
                response.ContentType = "application/json";
                response.Write(data.Json);
                return;
            }

            if (responseIsImage)
            {
                response.ContentType = "image/png";
                response.BinaryWrite(data.Image);
                return;
            }

            // internal error and fallback

            response.ContentType = "application/json";
            response.StatusCode = 500;
            var err = new {
                Error = $"Oops... Something went wrong (trackingId='{trackingId}')"
            };

            response.Write(err.ToJson());
        }

        private void startRequest(ISrvRequest request)
        {
            var http = request.Http.Request;
            var api = Api.Instance;

            switch (request.Route)
            {
                case "/thumbnail": {
                    api.Thumbnail(request);
                    break;
                }
                case "/config": {
                    var config = http.RequireArgs<SrvConfig>();
                    api.SetConfig(config);
                    writeResponse(new AnyResponse { Json = config.ToJson() });
                    endRequest();
                    break;
                }
                case "/log": {
                    var dump = Logger.Instance.Dump();
                    writeResponse(new AnyResponse { Json = dump.ToJson() });
                    endRequest();
                    break;
                }
                default: {
                    throw new ApplicationException($"Route '{request.Route}' is not found");
                }
            }
        }

        private void endRequest()
        {
            _isCompleted = true;
            _notifyCompletion(this);
        }

        #endregion

        #region interface

        bool IHttpHandler.IsReusable => false;
        void IHttpHandler.ProcessRequest(HttpContext http) { }
        bool IAsyncResult.IsCompleted => _isCompleted;
        WaitHandle IAsyncResult.AsyncWaitHandle => null;
        object IAsyncResult.AsyncState => null;
        bool IAsyncResult.CompletedSynchronously => false;

        IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext http, AsyncCallback cb, object extraData)
        {
            _log = Logger.Instance;
            _notifyCompletion = cb;

            var response = http.Response;
            var route = http.Request.FilePath.ToLowerInvariant();

            _request = SrvRequest.New(http, route, this);

            logStart(_request);

            response.Headers.Set("Access-Control-Allow-Origin", "*");

            try
            {
                startRequest(_request);
            }
            catch (Exception ex)
            {
                writeResponse(new AnyResponse { Error = ex });
                endRequest();
            }

            return this;
        }

        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
        {
            logEnd(_request);
        }

        void IAnyHandler.WriteResponse(AnyResponse data)
        {
            writeResponse(data);
        }

        void IAnyHandler.NotifyCompletion()
        {
            endRequest();
        }

        #endregion
    }
}