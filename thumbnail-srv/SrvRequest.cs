using System;
using System.Web;

namespace ThumbnailSrv
{
    class AnyResponse
    {
        public string Json { get; set; }
        public byte[] Image { get; set; }
        public Exception Error { get; set; }
    }

    interface ISrvRequest
    {
        HttpContext Http { get; }
        string Route { get; }
        string TrackingId { get; }
        void EndWith(byte[] image);
        void EndWith(string json);
        void EndWith(Exception error);
    }

    class SrvRequest : ISrvRequest
    {
        #region members

        private readonly HttpContext _http;
        private readonly string _route;
        private readonly IAnyHandler _handler;
        private readonly string _trackingId;

        #endregion

        #region construction

        public static ISrvRequest New(HttpContext http, string route, IAnyHandler handler)
        {
            return
                new SrvRequest(http, route, handler);
        }

        private SrvRequest(HttpContext http, string route, IAnyHandler handler)
        {
            _http = http;
            _route = route;
            _handler = handler;
            _trackingId = 3.GenerateId();
        }

        #endregion

        #region private
        #endregion

        #region interface

        HttpContext ISrvRequest.Http => _http;
        string ISrvRequest.Route => _route;
        string ISrvRequest.TrackingId => _trackingId;

        void ISrvRequest.EndWith(byte[] image)
        {
            _handler.WriteResponse(new AnyResponse { Image = image });
            _handler.NotifyCompletion();
        }

        void ISrvRequest.EndWith(string json)
        {
            _handler.WriteResponse(new AnyResponse { Json = json });
            _handler.NotifyCompletion();
        }

        void ISrvRequest.EndWith(Exception error)
        {
            _handler.WriteResponse(new AnyResponse { Error = error });
            _handler.NotifyCompletion();
        }

        #endregion
    }
}