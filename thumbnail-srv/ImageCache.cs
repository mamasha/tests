namespace ThumbnailSrv
{
    public enum CacheState
    {
        New, Pending, Ready
    }

    class CacheItem
    {
        public string Key { get; }
        public CacheState State { get; set; }
        public byte[] Image { get; set; }

        public CacheItem(string key)
        {
            Key = key;
        }
    }

    interface IImageCache
    {
        CacheItem Get(string key);
        void Put(string key, byte[] image);
    }

    class ImageCache : IImageCache
    {
        #region construction

        public static IImageCache New()
        {
            return 
                new ImageCache();
        }

        private ImageCache()
        { }

        #endregion

        #region interface

        CacheItem IImageCache.Get(string key)
        {
            return null;
        }

        void IImageCache.Put(string key, byte[] image)
        {
        }

        #endregion
    }
}