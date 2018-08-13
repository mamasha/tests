using System;
using System.Threading.Tasks;

namespace ThumbnailSrv
{
    delegate void OnSuccess<T>(T value);
    delegate void OnError(string key, Exception error);

    interface IAsyncFlow<T>
    {
        void WhenReady(string keyError, OnSuccess<T> onSuccess);
        bool RegisterSignal(string key);
        void Signal(string key, Task<T> task, OnError onError);
    }

    class AsyncFlow<T> : IAsyncFlow<T>
    {
        #region private

        #endregion

        #region interface

        void IAsyncFlow<T>.WhenReady(string key, OnSuccess<T> onSuccess)
        {
        }

        bool IAsyncFlow<T>.RegisterSignal(string key)
        {
            return true;
        }

        void IAsyncFlow<T>.Signal(string key, Task<T> task, OnError onError)
        {
        }
        #endregion
    }
}