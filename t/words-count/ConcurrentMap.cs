using System.Collections.Generic;

namespace words_count
{
    interface IConcurrentMap<Key, Value>
    {
        Value putIfAbsent(Key key, Value value);
        Key[] getKeys();
        Value this[Key key] { get; }
    }

    class ConcurrentMap<Key, Value> : IConcurrentMap<Key, Value>
    {
        #region members

        private readonly List<Key> _keys;
        private readonly Dictionary<Key, Value> _map;

        #endregion

        #region construction

        public static IConcurrentMap<Key, Value> New()
        {
            return
                AtomicConcurrentMap<Key, Value>.New(
                new ConcurrentMap<Key, Value>());
        }

        private ConcurrentMap()
        {
            _keys = new List<Key>();
            _map = new Dictionary<Key, Value>();
        }

        #endregion

        #region interface

        Value IConcurrentMap<Key, Value>.putIfAbsent(Key key, Value value)
        {
            if (_map.TryGetValue(key, out var previous))
                return previous;

            _map.Add(key, value);
            _keys.Add(key);

            return value;
        }

        Key[] IConcurrentMap<Key, Value>.getKeys()
        {
            return
                _keys.ToArray();
        }

        Value IConcurrentMap<Key, Value>.this[Key key]
        {
            get
            {
                if (_map.TryGetValue(key, out var value))
                    return value;

                return default(Value);
            }
        }

        #endregion
    }

    class AtomicConcurrentMap<Key, Value> : IConcurrentMap<Key, Value>
    {
        private readonly object _mutex = new object();
        private readonly IConcurrentMap<Key, Value> _;

        public static IConcurrentMap<Key, Value> New(IConcurrentMap<Key, Value> target)
        {
            return
                new AtomicConcurrentMap<Key, Value>(target);
        }

        private AtomicConcurrentMap(IConcurrentMap<Key, Value> target) { _ = target; }

        Value IConcurrentMap<Key, Value>.putIfAbsent(Key key, Value value) { lock (_mutex) return _.putIfAbsent(key, value); }
        Key[] IConcurrentMap<Key, Value>.getKeys() { lock(_mutex) return _.getKeys(); }
        Value IConcurrentMap<Key, Value>.this[Key key] { get { lock (_mutex) return _[key]; } }
    }
}