using System;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace FbApi
{
    [Serializable]
    class FbConfig
    {
        public string ApiBaseUrl { get; set; }
        public string ApiVer { get; set; }
        public string AppId { get; set; }
        public string RedirectUrl { get; set; }
        public string RequiredPermissions { get; set; }
        public TimeSpan WaitSpan { get; set; }

        [JsonIgnore]
        [ScriptIgnore]
        [NonSerialized]
        public string AppSecret;
    }

    [Serializable]
    class FbError
    {
        public Error error { get; set; }

        public class Error
        {
            public string message { get; set; }
            public string type { get; set; }
            public int code { get; set; }
            public int error_subcode { get; set; }
            public string error_user_title { get; set; }
            public string error_user_msg { get; set; }
            public string fbtrace_id { get; set; }
        }
    }

    [Serializable]
    class FbPage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Permissions { get; set; }
        public string Token { get; set; }
    }

    [Serializable]
    class FbUser
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public FbPage[] Pages { get; set; }
    }

    [Serializable]
    class FbPost
    {
        public string Id { get; set; }
    }

    [Serializable]
    class FbPhoto
    {
        public string Id { get; set; }
    }

    [Serializable]
    class UpPhoto
    {
        public int uploadId { get; set; }
        public string photoId { get; set; }
        public string pageId { get; set; }
        public int length { get; set; }
        public string fileName { get; set; }
        public string contentType { get; set; }
    }
}