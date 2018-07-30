using System;
using Newtonsoft.Json;

namespace FbApi
{
    [Serializable]
    [JsonObject(ItemRequired = Required.Always)]
    class PostRqst
    {
        public string pageId { get; set; }
        public string message { get; set; }

        [JsonProperty(Required = Required.Default)]
        public int[] uploadIds { get; set; }
    }

    class PublishPostRply
    { }

    [Serializable]
    [JsonObject(ItemRequired = Required.Always)]
    class PhotoRqst
    {
        public int uploadId { get; set; }
        public string pageId { get; set; }
        public string fileName { get; set; }
        public string fileType { get; set; }
        public int fileSize { get; set; }
    }

    class UploadPhotoRply
    { }

    interface IPostMaker
    { }

    class PostMaker
    { }
}