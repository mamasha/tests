using System.Threading.Tasks;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public interface IRelay
        {
            Task MapSession(MapSessionMsg msg);
            Task MapFlow(MapFlowMsg msg);
            Task UserIsHere(UserIsHereMsg msg);
            Task ClientIsGone(ClientIsGoneMsg msg);
            Task Notify(NotifyMsg msg);
        }

        class Relay : IRelay
        {
            private readonly ILog _log;
            private readonly IRelayRepo _repo;
            private readonly IIoOut _ioOut;
            private readonly IBus _bus;

            public Relay(
                ILog log,
                IRelayRepo repo,
                IIoOut ioOut,
                IBus bus)
            {
                _log = log;
                _repo = repo;
                _ioOut = ioOut;
                _bus = bus;
            }

            async Task IRelay.MapSession(MapSessionMsg msg)
            {
                _log.Info(msg);
                _repo.AddSession(msg.TrackingId, msg.SessionId);
                await Task.Yield();
            }

            async Task IRelay.MapFlow(MapFlowMsg msg)
            {
                _log.Info(msg);
                _repo.AddFlow(msg.FlowId, msg.TrackingId);
                await Task.Yield();
            }

            async Task IRelay.UserIsHere(UserIsHereMsg msg)
            {
                var user = msg.To<User>();
                _log.Info(user);

                _repo.AddUser(msg.SessionId, user);

                await Task.Yield();
            }

            async Task IRelay.ClientIsGone(ClientIsGoneMsg msg)
            {
                _log.Info(msg);

                _repo.RemoveUser(msg.SessionId);

                await Task.Yield();
            }

            async Task IRelay.Notify(NotifyMsg msg)
            {
                if (!_repo.TryGetFlow(msg.FlowId, out var trackingId))
                {
                    _log.Warn("No flow is here", msg);
                    return;
                }

                if (!_repo.TryGetSession(trackingId, out var sessionId))
                {
                    _log.Warn("No session is here", msg);
                    return;
                }

                if (!_repo.TryGetUser(sessionId, out var user))
                {
                    _log.Warn("No user is here", msg);
                    return;
                }

                var pushMsg = msg.To<PushMsg>(
                    x => x.ConnectionId = user.ConnectionId);

                _log.Info(new { msg, pushMsg });

                await _bus.Call(_ioOut.Push, pushMsg);
            }

            internal class Null : IRelay
            {
                Task IRelay.MapSession(MapSessionMsg msg) { return Task.CompletedTask; }
                Task IRelay.MapFlow(MapFlowMsg msg) { return Task.CompletedTask; }
                Task IRelay.Notify(NotifyMsg msg) { return Task.CompletedTask; }
                Task IRelay.UserIsHere(UserIsHereMsg msg) { return Task.CompletedTask; }
                Task IRelay.ClientIsGone(ClientIsGoneMsg msg) { return Task.CompletedTask; }
            }
        }
    }
}
