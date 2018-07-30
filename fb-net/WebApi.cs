using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FbApi
{
    delegate void OnHttpError(WebApiExceptionDetail detail);

    interface IWebApi
    {
        T Get<T>(string url, OnHttpError handler, T proto = null) where T : class;
        T Post<T>(string url, OnHttpError handler, HttpContent content = null, T proto = null) where T : class;
    }

    class WebApi : IWebApi
    {
        #region members

        private readonly ILogger _log;
        private readonly HttpClient _http;

        #endregion

        #region construction

        public static IWebApi New(ILogger log)
        {
            return
                new WebApi(log);
        }

        private WebApi(ILogger log)
        {
            _log = log;
            _http = new HttpClient();
        }

        #endregion

        #region private

        private void throwOnErrorStatus(OnHttpError handler, HttpResponseMessage response, string reply)
        {
            if (response.IsSuccessStatusCode)
                return;

            var request = response.RequestMessage;
            var statusCode = (int) response.StatusCode;

            var requestHeaders =
                from header in request.Headers
                let values = header.Value.Select(value => $"'{value}'")
                select $"{header.Key}: {string.Join(";", values)}";

            var responseHeaders =
                from header in response.Headers
                let values = header.Value.Select(value => $"'{value}'")
                select $"{header.Key}: {string.Join(";", values)}";

            var detail = new WebApiExceptionDetail {
                statusCode = response.StatusCode,
                isSuccessfull = response.IsSuccessStatusCode,
                reason = response.ReasonPhrase,
                request = new WebApiExceptionDetail.Request {
                    method = request.Method.Method,
                    url = request.RequestUri.ToString(),
                    headers = requestHeaders.ToArray()
                },
                response = new WebApiExceptionDetail.Response {
                    headers = responseHeaders.ToArray(),
                    text = reply
                }
            };

            handler?.Invoke(detail);

            throw 
                new WebApiException($"Bad http status code {statusCode}", detail);
        }

        private T fetch<T>(string url, OnHttpError handler, T proto, Func<Task<HttpResponseMessage>> method)
        {
            _log.Trace($"    fetching: '{url}'");
            var response = method().Result;
            var json = response.Content.ReadAsStringAsync().Result;
            _log.Trace($"    got: '{json}'");

            throwOnErrorStatus(handler, response, json);

            if (proto != null)
                return JsonConvert.DeserializeAnonymousType(json, proto);

            return 
                JsonConvert.DeserializeObject<T>(json);
        }

        #endregion

        #region interface

        T IWebApi.Get<T>(string url, OnHttpError handler, T proto)
        {
            return 
                fetch(url, handler, proto, () => _http.GetAsync(url));
        }

        T IWebApi.Post<T>(string url, OnHttpError handler, HttpContent content, T proto)
        {
            return
                fetch(url, handler, proto, () => _http.PostAsync(url, content));
        }

        #endregion
    }
}