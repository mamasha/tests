using System;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FbApi
{
    static class HttpHelpers
    {
        public static bool IsNotEmpty(this string str)
        {
            return
                !String.IsNullOrWhiteSpace(str);
        }

        public static bool IsEmpty(this string str)
        {
            return
                String.IsNullOrWhiteSpace(str);
        }

        public static string ToJson(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return json;
        }

        public static T Parse<T>(this string json)
        {
            return
                JsonConvert.DeserializeObject<T>(json);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return
                TimeSpan.FromSeconds(seconds);
        }

        public static string Require(this HttpRequest request, string name)
        {
            var value = request[name];
            if (String.IsNullOrWhiteSpace(value))
                throw new ApplicationException($"Required parameter '{name}' is not found in request");
            return value;
        }

        public static T Require<T>(this HttpRequest request, string token = "args")
        {
            var json = request.Require(token);
            var data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }

        public static object Dump(this HttpRequest request)
        {
            string[] ignoreList = { "ALL_HTTP", "ALL_RAW", "QUERY_STRING" };

            var cookies = request.Cookies.AllKeys;
            var queryKeys = request.QueryString.AllKeys;
            var formKeys = request.Form.AllKeys;
            var srvKeys = request.ServerVariables.AllKeys;

            var args =
                from string key in request.Params
                where !ignoreList.Contains(key)
                let src =
                    cookies.Contains(key) ? "C" :
                    queryKeys.Contains(key) ? "Q" :
                    formKeys.Contains(key) ? "F" :
                    srvKeys.Contains(key) ? "S" : "?"
                let value = request.Params[key]
                where !String.IsNullOrWhiteSpace(value)
                select $"[{src}] {key}: '{value}'";

            var headers =
                from string hdr in request.Headers
                where hdr.ToLower() != "cookie"
                select $"{hdr}: '{request.Headers[hdr]}'";

            var dump = new {
                request.ApplicationPath,
                request.AppRelativeCurrentExecutionFilePath,
                //                rq.Browser,
                //                rq.ContentEncoding,
                request.ContentLength,
                request.ContentType,
                //                rq.Cookies,
                request.CurrentExecutionFilePath,
                request.FilePath,
                request.Form,
                Headers = headers.ToArray(),
                request.HttpMethod,
                request.IsAuthenticated,
                request.IsLocal,
                request.IsSecureConnection,
                //                rq.Params,
                request.Path,
                request.PhysicalApplicationPath,
                request.PhysicalPath,
                //                rq.QueryString,
                request.RawUrl,
                request.RequestType,
                //                rq.ServerVariables,
                request.TotalBytes,
                request.Url,
                request.UrlReferrer,
                request.UserAgent,
                request.UserHostAddress,
                request.UserHostName,
                request.UserLanguages,
                Args = args.ToArray()
            };

            return dump;
        }
    }
}