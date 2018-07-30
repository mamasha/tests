using System;
using System.Net;
using System.Text;
using System.Web;

namespace FbApi
{
    public class AnyHandler : IHttpHandler
    {
        #region private

        private void logStart(string sessionId, HttpRequest request, AnyResult result)
        {
            var log = AppController.Instance.Logger;
            log.Trace($"{result.trackingId} << {request.RawUrl}");
        }

        private void logEnd(string sessionId, HttpRequest request, AnyResult result, Exception error)
        {
            var log = AppController.Instance.Logger;

            var sb = new StringBuilder(1024);

            sb.Append($"{result.trackingId} >> ");

            if (error == null)
            {
                sb.Append(result.ToWireJson());
                log.Trace(sb.ToString());
                return;
            }

            var details = new {
                errorMessage = error.Message,
                sessionId,
                error.StackTrace,
                result,
                request = request.Dump(),
            };

            sb.Append(details.ToJson());
            log.Trace(sb.ToString());
        }

        private AnyResult newResult(string sessionId)
        {
            var result = new AnyResult {
                trackingId = AnyResultHelpers.GenerateTrackingId(sessionId),
                status = HttpStatusCode.OK
            };
            return result;
        }

        private void setErrorDetails(AnyResult result, Exception error, bool detailsOnError)
        {
            if (error == null)
                return;

            result.status = HttpStatusCode.InternalServerError;
            result.error = new AnyResult.Error {
                message = $"Something went wrong ({result.trackingId})",
            };

            if (detailsOnError == false)
                return;

            result.error.message += $"; {error.Message}";
            result.error.details = error.Data;
        }

        private object routeRequest(IAppController ctrl, string route, HttpContext http)
        {
            var request = http.Request;
            object result;

            switch (route)
            {
                case "/session/init": {
                    var sessionId = request.Require("sessionId");
                    result = ctrl.Init(sessionId);
                    break;
                }
                case "/session/connect": {
                    var sessionId = request.Require("sessionId");
                    result = ctrl.Connect(sessionId);
                    break;
                }
                case "/fb-accepted": {
                    var sessionId = request.Require("state");
                    var code = request.Require("code");
                    result = ctrl.ConnectionAccepted(sessionId, code);
                    break;
                }
                case "/session/connect/wait": {
                    var sessionId = request.Require("sessionId");
                    result = ctrl.WaitForConnection(sessionId);
                    break;
                }
                case "/session/disconnect": {
                    var sessionId = request.Require("sessionId");
                    result = ctrl.Disconnect(sessionId);
                    break;
                }
                case "/photo/upload": {
                    var sessionId = request.Require("sessionId");
                    var photo = request.Require<PhotoRqst>();
                    result = ctrl.UploadPhoto(sessionId, photo, http);
                    break;
                }
                case "/post/publish": {
                    var sessionId = request.Require("sessionId");
                    var post = request.Require<PostRqst>();
                    result = ctrl.PublishPost(sessionId, post);
                    break;
                }
                default:
                    throw new ApplicationException($"Route '{route}' is not found");
            }

            return result;
        }

        private string getSessionId(HttpRequest request)
        {
            var sessionId = request["sessionId"];

            if (sessionId.IsEmpty())
                sessionId = request["state"];

            if (sessionId.IsEmpty())
                sessionId = "--.--.--";

            return sessionId;
        }

        #endregion

        #region interface

        bool IHttpHandler.IsReusable => true;

        void IHttpHandler.ProcessRequest(HttpContext http)
        {
            var request = http.Request;
            var response = http.Response;
            var route = request.FilePath.ToLowerInvariant();

            var sessionId = getSessionId(request);
            var result = newResult(sessionId);
            logStart(sessionId, request, result);

            response.ContentType = "text/plain";
            response.Headers.Set("Access-Control-Allow-Origin", "*");

            Exception error = null;

            try
            {
                var ctrl = AppController.Instance;
                result.api = routeRequest(ctrl, route, http);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int) HttpStatusCode.InternalServerError;
                error = ex;
            }

            setErrorDetails(result, error, true);
            logEnd(sessionId, request, result, error);

            if (route == "/fb-accepted")
            {
                response.ContentType = "text/HTML";
                response.Write(result.api);
                return;
            }

            response.Write(result.ToJson());
        }

        #endregion
    }
}