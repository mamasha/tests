using System;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace ThumbnailSrv
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
            var value = request.QueryString[name];
            if (String.IsNullOrWhiteSpace(value))
                throw new ApplicationException($"Required parameter '{name}' is not found in request");
            return value;
        }

        public static int RequireInt(this HttpRequest request, string name)
        {
            var str = request[name];
            if (String.IsNullOrWhiteSpace(str))
                throw new ApplicationException($"Required parameter '{name}' is not found in request");
            if (!int.TryParse(str, out int value))
                throw new ApplicationException($"Required parameter '{name}' is not a number; value='{str}'");
            return value;
        }

        public static T RequireArgs<T>(this HttpRequest request, string token = "args")
        {
            var json = request.Require(token);
            var data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }

        public static int OptionalInt(this HttpRequest request, string name, int defaultValue)
        {
            var str = request[name];
            if (String.IsNullOrWhiteSpace(str))
                return defaultValue;
            if (!int.TryParse(str, out int value))
                throw new ApplicationException($"Optional parameter '{name}' is not a number; value='{str}'");
            return value;
        }

        private static readonly Random _random = new Random();

        public static string GenerateId(this int segmentCount)
        {
            var sb = new StringBuilder();
            var first = _random.Next(10, 100).ToString();

            sb.Append(first);
            segmentCount--;

            while (segmentCount-- > 0)
            {
                var next = _random.Next(0, 100).ToString("00");
                sb
                    .Append(".")
                    .Append(next);
            }

            return
                sb.ToString();
        }


    }
}