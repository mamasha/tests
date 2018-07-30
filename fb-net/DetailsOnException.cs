using System;

namespace FbApi
{
    static class DetailsOnExceptionHelper
    {
        public static void AddDetail(this Exception ex, string key, object detail)
        {
            if (detail == null)
                return;

            if (detail.GetType().IsSerializable)
            {
                ex.Data[key] = detail;
                return;
            }

            ex.Data[key] = detail.ToJson();
        }

        public static void AddDetail<T>(this Exception ex, T detail)
            where T : class
        {
            if (detail == null)
                return;

            var key = detail.GetType().Name;

            if (detail.GetType().IsSerializable)
            {
                ex.Data[key] = detail;
                return;
            }

            ex.Data[key] = detail.ToJson();
        }
    }

    sealed class DetailsOnException
    {
        public void tail<T1>(T1 d1, Action impl) 
            where T1 : class
        {
            try
            {
                impl();
            }
            catch (Exception ex)
            {
                ex.AddDetail(d1);
                throw;
            }
        }

        public void tail<T1, T2>(T1 d1, T2 d2, Action impl)
            where T1 : class
            where T2 : class
        {
            try
            {
                impl();
            }
            catch (Exception ex)
            {
                ex.AddDetail(d1);
                ex.AddDetail(d2);
                throw;
            }
        }

        public R tail<T1, R>(T1 d1, Func<R> impl)
            where T1 : class
        {
            try
            {
                return impl();
            }
            catch (Exception ex)
            {
                ex.AddDetail(d1);
                throw;
            }
        }

        public R tail<T1, T2, R>(T1 d1, T2 d2, Func<R> impl)
            where T1 : class
            where T2 : class
        {
            try
            {
                return impl();
            }
            catch (Exception ex)
            {
                ex.AddDetail(d1);
                ex.AddDetail(d2);
                throw;
            }
        }
    }
}