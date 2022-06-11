using System;
using System.Threading.Tasks;
using Quali.Colony.Services.Common.OneToOneOrMany;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        [MvcRpc]
        class AMsg
        {}

        class BMsg
        {}

        interface IFsm
        {
            IFsm _(Action impl);
            IFsm _(Func<Task> impl);

            IFsm __<TIn, TOut>(Func<TIn, Task<TOut>> impl);
        }

        interface IAmplitudeFsm
        {
            Task Push(AMsg msg);
            Task Push2(AMsg msg);
        }

        class AmplitudeFsm : IAmplitudeFsm
        {
            private readonly ILog _log;
            private readonly IFsm _fsm;
            private readonly IBus _bus;

            public AmplitudeFsm(ILog log, IFsm fsm, IBus bus)
            {
                _log = log;
                _fsm = fsm;
                _bus = bus;
            }

            private async Task<BMsg> authenticate(int i, AMsg msg)
            {
                await Task.Yield();
                return new BMsg();
            }

            private async Task setUser(int i, BMsg msg)
            {
                await Task.Yield();
            }

            private async Task sendToAmplify(int i, BMsg msg)
            {
                _log.Info(new { i });
                await Task.Yield();
            }

            async Task IAmplitudeFsm.Push(AMsg msg)
            {
                var i = 10;
                BMsg bMsg = null;

                _fsm                                      ._(async () =>
                    bMsg = await authenticate(i, msg)    )._(() =>
                    setUser(i, bMsg)                     )._(() =>
                    sendToAmplify(i, bMsg)               );

                await Task.Yield();
            }

            private async Task<BMsg> aaa(AMsg msg)
            {
                await Task.Yield();
                return new BMsg();
            }

            private async Task<string> authenticate(string apiKey)
            {
                await Task.Yield();
                return apiKey;
            }

            private async Task<BMsg> authenticate2(AMsg msg)
            {
                await Task.Yield();
                return new BMsg();
            }

            async Task IAmplitudeFsm.Push2(AMsg msg)
            {
                var apiKey = "1234";

                var res = await _bus.Call(authenticate2, msg);
                var userId = await _bus.Call(authenticate, apiKey);
                await Task.Yield();
            }
        }
    }
}
