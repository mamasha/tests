using System;
using System.Collections.Generic;

namespace ThumbnailSrv
{
    interface ILocalCache<T>
        where T: class
    {
        (T, bool) Get(string key);
        bool Put(string key, T value);
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

        (T, bool) ILocalCache<T>.Get(string key)
        {
            if (_db.TryGetValue(key, out T value))
            {
                return
                    (value, false);
            }

            _db.Add(key, null);

            return 
                (null, true);
        }

        bool ILocalCache<T>.Put(string key, T value)
        {
            var firstTouch = !_db.ContainsKey(key);
            _db[key] = value;

            return firstTouch;
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

        (T, bool) ILocalCache<T>.Get(string key) { lock(_mutex) return _peer.Get(key); }
        bool ILocalCache<T>.Put(string key, T value) { lock (_mutex) return _peer.Put(key, value); }
    }
}