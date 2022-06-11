using System;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Quali.Colony.Abstractions.Context;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        static void RunAsync(Func<Task> action, Action<Exception> onError = null)
        {
            async Task me()
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }
            }

            Task.Run(me);
        }
    }

    static class DevXHubInternalHelpers
    {
        public static T To<T>(this object self)
            where T : class, new()
        {
            return Mapper.Map<T>(self);
        }

        public static T To<T>(this object self, Action<T> set)
            where T : class, new()
        {
            var dst = Mapper.Map<T>(self);
            set(dst);
            return dst;
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }
    }

    public static class DevXHubHelpers
    {
        public static async Task MapMySession(this IDevXHub devXHub, string sessionId)
        {
            var trackingId = OperationContext.Current.TraceId;
            await devXHub.MapSession(trackingId, sessionId);
        }

        public static async Task MapMyFlow<TTag>(this IDevXHub devXHub, string flowId)
            where TTag : DevXHub.ITag
        {
            var trackingId = OperationContext.Current.TraceId;
            var flowName = typeof(TTag).Name;

            await devXHub.MapFlow(trackingId, flowName, flowId);
        }
    }
}
