using System;
using System.Threading.Tasks;

namespace ThumbnailSrv
{
    delegate bool GotoState<T>(T data);

    interface IAsyncFlow<T>
    {
        void WhenReady(string key, GotoState<T> gotoNext);
        void Signal(string key, Task<T> task);
    }

    class AsyncFlow<T> : IAsyncFlow<T>
    {
        #region private

        #endregion

        #region interface

        void IAsyncFlow<T>.WhenReady(string key, GotoState<T> gotoNext)
        {
        }

        void IAsyncFlow<T>.Signal(string key, Task<T> task)
        {
        }
        #endregion
    }

    static class AsyncFlowHelpers
    {
        public static bool Next<T>(this IAsyncFlow<T> async, GotoState<T> gotoState, CacheItem<T> item)
        {
            switch (item.State)
            {
                case CacheState.New:
                    async.WhenReady(item.Key, gotoState);
                    return true;

                case CacheState.Pending:
                    async.WhenReady(item.Key, gotoState);
                    return false;

                case CacheState.Ready:
                    gotoState(item.Value);
                    return false;

                default:
                    throw new Exception("Should not get here");
            }
        }
    }
}