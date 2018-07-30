using System;
using System.Net;

namespace FbApi
{
    [Serializable]
    public class WebApiExceptionDetail
    {
        public HttpStatusCode statusCode { get; set; }
        public bool isSuccessfull { get; set; }
        public string reason { get; set; }
        public Request request { get; set; }
        public Response response { get; set; }

        public class Request
        {
            public string method { get; set; }
            public string url { get; set; }
            public string[] headers { get; set; }
        }

        public class Response
        {
            public string[] headers;
            public string text;
        }
    }

    sealed class WebApiException : Exception
    {
        private static readonly string _myKey = nameof(WebApiException);

        public WebApiExceptionDetail Detail { get; }

        public WebApiException(string message, WebApiExceptionDetail detail)
            : base(message)
        {
            Detail = detail;
            Data[_myKey] = detail;
        }
    }
}