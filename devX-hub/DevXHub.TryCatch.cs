using System;
using System.Threading.Tasks;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        static class Try
        {
            public static class Catch
            {
                public static async Task Handle(Func<Task> action, Action<Exception> onError)
                {
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        onError(ex);
                    }
                }

                public static async Task Rethrow(Func<Task> action, Action<Exception> onError)
                {
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        onError(ex);
                        throw;
                    }
                }
            }
        }
    }
}
