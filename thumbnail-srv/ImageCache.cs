namespace ThumbnailSrv
{
    public enum CacheState
    {
        New, Pending, Ready
    }

    class CacheItem<T>
    {
        public string Key { get; }
        public CacheState State { get; set; }
        public T Value { get; set; }

        public CacheItem(string key)
        {
            Key = key;
        }
    }

    interface IImageCache<T>
    {
        CacheItem<T> Get(string key);
        void Put(string key, T value);
    }

    class ImageCache<T> : IImageCache<T>
    {
        #region construction

        public static IImageCache<T> New()
        {
            return 
                new ImageCache<T>();
        }

        private ImageCache()
        { }

        #endregion

        #region interface

        CacheItem<T> IImageCache<T>.Get(string key)
        {
            return null;
        }

        void IImageCache<T>.Put(string key, T image)
        {
        }

        #endregion
    }
}