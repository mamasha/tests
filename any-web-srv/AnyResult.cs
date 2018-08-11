using System;
using System.Collections;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace ThumbnailSrv
{
    class AnyResult
    {
        public string trackingId { get; set; }
        public HttpStatusCode status { get; set; }
        public Error error { get; set; }
        public object api { get; set; }

        public class Error
        {
            public string message { get; set; } 
            public IDictionary details { get; set; }
        }
    }

    static class AnyResultHelpers
    {
        private static readonly Random _random = new Random();

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string ToWireJson(this AnyResult result)
        {
            return
                JsonConvert.SerializeObject(result, Formatting.None, _jsonSettings);
        }

        public static string ToJson(this AnyResult result)
        {
            return
                JsonConvert.SerializeObject(result, Formatting.Indented, _jsonSettings);
        }

        private static string generateId(int segmentCount)
        {
            var sb = new StringBuilder();

            sb.Append(_random.Next(10, 100).ToString());
            segmentCount--;

            while (segmentCount-- > 0)
                sb.Append(_random.Next(0, 100).ToString("00"));

            return
                sb.ToString();
        }

        public static string GenerateTrackingId(string sessionId)
        {
            if (sessionId.IsEmpty())
                return generateId(6);

            var suffix = generateId(3);

            return
                $"{sessionId}.{suffix}";
        }
    }
}