﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThumbnailSrv
{
    delegate void OnReady<T>(T value, Exception error);

    interface IAsyncFlow<T>
        where T: class
    {
        bool TouchPoint(string key);
        void WhenReady(string key, OnReady<T> onReady);
        void WaitFor(string key, Task<T> task);
        void DoneWith(string key);
    }

    class AsyncFlow<T> : IAsyncFlow<T>
        where T : class
    {
        #region members

        class ExecutionPoint
        {
            public Queue<OnReady<T>> Que { get; set; }
            public T Value { get; set; }
            public Exception Error { get; set; }
            public int TouchCount;
        }

        private readonly ILogger _log;
        private readonly Dictionary<string, ExecutionPoint> _db;

        #endregion

        #region private

        #endregion

        #region construction

        public static IAsyncFlow<T> New(ILogger log)
        {
            return 
                new AsyncFlow<T>(log);
        }

        private AsyncFlow(ILogger log)
        {
            _log = log;
            _db = new Dictionary<string, ExecutionPoint>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region private

        private ExecutionPoint getMyPoint(string key)
        {
            lock (_db)
            {
                if (_db.TryGetValue(key, out var point))
                    return point;

                point = new ExecutionPoint {
                    Que = new Queue<OnReady<T>>()
                };

                _db.Add(key, point);

                return point;
            }
        }

        private void invoke(OnReady<T> onReady, T value, Exception error)
        {
            try
            {
                onReady(value, error);
            }
            catch (Exception ex)
            {
                _log.error("--.--.--", "async-flow", ex, "While executing onReady callback");
            }
        }

        private void enqueue(ExecutionPoint point, OnReady<T> onReady)
        {
            T value;
            Exception error;

            for (;;)
            {
                lock (point)
                {
                    value = point.Value;
                    error = point.Error;

                    if (value != null)
                        break;

                    if (error != null)
                        break;

                    point.Que.Enqueue(onReady);
                    return;
                }
            }

            invoke(onReady, value, error);
        }

        private void complete(ExecutionPoint point, T value, Exception error)
        {
            Trace.Assert(value != null || error != null);

            OnReady<T>[] handlers;

            lock (point)
            {
                point.Value = value;
                point.Error = error;

                handlers = point.Que.ToArray();
                point.Que.Clear();
            }

            foreach (var onReady in handlers)
            {
                invoke(onReady, value, error);
            }
        }

        #endregion

        #region interface

        bool IAsyncFlow<T>.TouchPoint(string key)
        {
            var point = getMyPoint(key);

            var touchCount = Interlocked.Increment(ref point.TouchCount);

            return
                touchCount == 1;        // return true on first touch
        }

        void IAsyncFlow<T>.WhenReady(string key, OnReady<T> onReady)
        {
            var point = getMyPoint(key);
            var value = point.Value;

            if (value != null)
            {
                invoke(onReady, value, null);
                return;
            }

            enqueue(point, onReady);
        }

        void IAsyncFlow<T>.WaitFor(string key, Task<T> task)
        {
            var point = getMyPoint(key);

            void signal(Task<T> completed)
            {
                try
                {
                    var value = completed.Result;
                    Trace.Assert(value != null);

                    complete(point, value, null);
                }
                catch (Exception ex)
                {
                    _log.info("--.--.--", "async-flow", () => $"{key}: task was completed with error; {ex.Message}");
                    complete(point, null, ex);
                }
            }

            task.ContinueWith(signal);
        }

        void IAsyncFlow<T>.DoneWith(string key)
        {
        }

        #endregion
    }
}