using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        interface IIoOut
        {
            Task Push(PushMsg msg);
        }

        class IoOut : IIoOut
        {
            private readonly ILog _log;
            private readonly IIoRepo _repo;
            private readonly IHubContext<IoIn> _signalR;

            public IoOut(ILog log, IIoRepo repo, IHubContext<IoIn> signalR)
            {
                _log = log;
                _repo = repo;
                _signalR = signalR;
            }

            async Task IIoOut.Push(PushMsg msg)
            {
                _log.Info(msg);

                if (!_repo.ItsMyConnection(msg.ConnectionId))
                    return;

                var io = _signalR.Clients.Client(msg.ConnectionId);

                if (io == null)
                {
                    _log.Warn("Connection is not found", new { msg.ConnectionId });
                    return;
                }

                var json = msg.Data.ToJson();

                await io.SendAsync("push", json);
            }

            public class Null : IIoOut
            {
                Task IIoOut.Push(PushMsg msg) { return Task.CompletedTask; }
            }
        }
    }
}
