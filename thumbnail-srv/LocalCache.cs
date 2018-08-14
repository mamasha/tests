using System;
using System.Collections.Generic;

namespace ThumbnailSrv
{
    public enum CacheState
    {
        New, Pending, Ready
    }

    class CacheItem<T>
        where T: class
    {
        public string Key { get; }
        public CacheState State { get; set; }
        public T Value { get; set; }

        public CacheItem(string key)
        {
            Key = key;
        }
    }

    interface ILocalCache<T>
        where T: class
    {
        (T, bool) Get2(string key);
        CacheItem<T> Get(string key);
        void Put(string key, T value);
    }

    class LocalCache<T> : ILocalCache<T>
        where T : class
    {
        #region members

        private readonly Dictionary<string, T> _db;

        #endregion

        #region construction

        public static ILocalCache<T> New()
        {
            return 
                SyncedLocalCache<T>.New(
                new LocalCache<T>());
        }

        private LocalCache()
        {
            _db = new Dictionary<string, T>(StringComparer.CurrentCultureIgnoreCase);
        }

        #endregion

        #region interface

        CacheItem<T> ILocalCache<T>.Get(string key)
        {
            var item = new CacheItem<T>(key);

            if (!_db.TryGetValue(key, out T value))
            {
                item.State = CacheState.New;
                _db.Add(key, null);
                return item;
            }

            item.Value = value;

            item.State =
                value != null ? CacheState.Ready : CacheState.Pending;

            return item;
        }

        void ILocalCache<T>.Put(string key, T value)
        {
            _db[key] = value;
        }

        #endregion
    }

    class SyncedLocalCache<T> : ILocalCache<T>
        where T : class
    {
        private readonly object _mutex = new object();
        private readonly ILocalCache<T> _peer;

        public static ILocalCache<T> New(ILocalCache<T> peer) { return new SyncedLocalCache<T>(peer); }
        private SyncedLocalCache(ILocalCache<T> peer) { _peer = peer; }

        CacheItem<T> ILocalCache<T>.Get(string key) { lock(_mutex) return _peer.Get(key); }
        void ILocalCache<T>.Put(string key, T value) { lock (_mutex) _peer.Put(key, value); }
    }
}