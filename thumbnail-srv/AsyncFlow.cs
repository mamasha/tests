using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThumbnailSrv
{
    delegate void OnSuccess<T>(T value);
    delegate void OnError(string key, Exception error);

    interface IAsyncFlow<T>
        where T: class
    {
        void WhenReady(string key, OnSuccess<T> onSuccess);
        bool TouchSignal(string key);
        void Signal(string key, Task<T> task, OnError onError);
        void DoneWith(string key);
    }

    class AsyncFlow<T> : IAsyncFlow<T>
        where T : class
    {
        #region members

        class ExecutionPoint
        {
            public Queue<OnSuccess<T>> Que { get; set; }
            public T Value { get; set; }
            public bool IsRegistered { get; set; }
            public OnError ErrorHandler { get; set; }
        }

        private readonly Dictionary<string, ExecutionPoint> _db;

        #endregion

        #region private

        #endregion

        #region construction

        public static IAsyncFlow<T> New()
        {
            return 
                new AsyncFlow<T>();
        }

        private AsyncFlow()
        {
            _db = new Dictionary<string, ExecutionPoint>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region private

        private ExecutionPoint getMyPoint(string key)
        {
            if (_db.TryGetValue(key, out var point))
                return point;

            point = new ExecutionPoint {
                Que = new Queue<OnSuccess<T>>()
            };

            _db.Add(key, point);

            return point;
        }

        void signal(string key)
        {
            
        }

        #endregion

        #region interface

        void IAsyncFlow<T>.WhenReady(string key, OnSuccess<T> onSuccess)
        {
            var point = getMyPoint(key);

            if (point.Value != null)
            {
                onSuccess(point.Value);
                return;
            }

            point.Que.Enqueue(onSuccess);
        }

        bool IAsyncFlow<T>.RegisterSignal(string key)
        {
            var point = getMyPoint(key);
            var firstRegistration = !point.IsRegistered;

            point.IsRegistered = true;
        }

        void IAsyncFlow<T>.Signal(string key, Task<T> task, OnError onError)
        {
            task.ContinueWith(_ => signal(key));
        }

        #endregion
    }
}