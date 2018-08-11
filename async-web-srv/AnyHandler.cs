using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace ThumbnailSrv
{
    public class AnyHandler : IHttpAsyncHandler, IAsyncResult
    {
        #region members

        private bool _isCompleted;
        private AnyRequest _my;

        #endregion

        #region private

        private void logStart(AnyRequest my)
        {
        }

        private void logEnd(AnyRequest my)
        {
        }

        private void writeResponse(AnyRequest my)
        {
            var response = my.Http.Response;

            if (my.InternalError != null)
            {
                response.ContentType = "application/json";
                response.StatusCode = 500;
                var data = new {
                    Error = my.InternalError.Message
                };
                response.Write(data.ToJson());
                return;
            }

            if (my.UserError != null)
            {
                response.ContentType = "application/json";
                response.StatusCode = 400;
                var data = new {
                    Error = my.UserError
                };
                response.Write(data.ToJson());
                return;
            }

            if (my.Json != null)
            {
                response.ContentType = "application/json";
                response.StatusCode = 400;
                response.Write(my.Json);
                return;
            }

            if (my.Image != null)
            {
                response.ContentType = "image/png";
                response.BinaryWrite(my.Image);
                return;
            }
        }

        private void startRequest(AnyRequest my)
        {
            var http = my.Http.Request;

            switch (my.Route)
            {
                case "/thumbnail": {
                    var width = http.RequireInt("width");
                    var height = http.RequireInt("height");
                    var url = http.Require("url");
                    break;
                }
                default: {
                    throw new ApplicationException($"Route '{my.Route}' is not found");
                }
            }
        }

        private void endRequest(AnyRequest my, AsyncCallback notifyCompletion)
        {
            writeResponse(my);
            _isCompleted = true;
            notifyCompletion(this);
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
            var request = http.Request;
            var response = http.Response;
            var route = request.FilePath.ToLowerInvariant();

            _my = new AnyRequest {
                Http = http,
                NotifyCompletion = () => endRequest(_my, cb),
                Route = route
            };

            logStart(_my);

            response.Headers.Set("Access-Control-Allow-Origin", "*");

            try
            {
                startRequest(_my);
            }
            catch (ApplicationException ex)
            {
                _my.UserError = ex.Message;
                endRequest(_my, cb);
            }
            catch (Exception ex)
            {
                _my.InternalError = ex;
                endRequest(_my, cb);
            }

            return this;
        }

        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
        {
            logEnd(_my);
        }


        #endregion
    }
}