using System;
using System.Web;

namespace ThumbnailSrv
{
    class AnyRequest
    {
        public HttpContext Http { get; set; }
        public Action NotifyCompletion { get; set; }
        public string Route { get; set; }
        public string Json { get; set; }
        public byte[] Image { get; set; }
        public string UserError { get; set; }
        public Exception InternalError { get; set; }
    }
}