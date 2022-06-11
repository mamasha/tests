using System;
using System.Threading.Tasks;
using Quali.Colony.Logging;

namespace Quali.Colony.Services.Common.devX_hub
{
    public interface IDevXHub
    {
        IJsonLogger Log { get; }
        Task MapSession(string trackingId, string sessionId);
        Task MapFlow(string trackingId, string flowName, string flowId);
        Task Notify(string flowId, string msg, object details);
    }

    public partial class DevXHub : IDevXHub
    {
        private readonly ILog _log;
        private readonly IIdFactory _idFactory;
        private readonly IRelay _relay;
        private readonly IBus _bus;

        public DevXHub(ILog log, IIdFactory idFactory, IRelay relay, IBus bus)
        {
            _log = log;
            _idFactory = idFactory;
            _relay = relay;
            _bus = bus;
        }

        IJsonLogger IDevXHub.Log => _log;

        async Task IDevXHub.MapSession(string trackingId, string sessionId)
        {
            var msg = new MapSessionMsg {
                MsgId = _idFactory.GetNextId<MapSessionMsg>(),
                TrackingId = trackingId,
                SessionId = sessionId
            };

            _log.Info(msg);

            await _bus.Call(_relay.MapSession, msg);
        }

        async Task IDevXHub.MapFlow(string trackingId, string flowName, string flowId)
        {
            var msg = new MapFlowMsg {
                MsgId = _idFactory.GetNextId<MapFlowMsg>(),
                TrackingId = trackingId,
                FlowName = flowName,
                FlowId = flowId
            };

            _log.Info(msg);

            await _bus.Call(_relay.MapFlow, msg);
        }

        async Task IDevXHub.Notify(string flowId, string text, object details)
        {
            var msg = new NotifyMsg {
                Id = _idFactory.GetNextId<NotifyMsg>(),
                FlowId = flowId,
                Data = new {
                    notification = new { msg = text, details }
                }
            };

            _log.Info(msg);

            await _bus.Call(_relay.Notify, msg);
        }

        public class Null : IDevXHub
        {
            IJsonLogger IDevXHub.Log => JsonLogger.Null;
            Task IDevXHub.MapSession(string trackingId, string sessionId) { return Task.CompletedTask; }
            Task IDevXHub.MapFlow(string trackingId, string flowName, string flowId) { return Task.CompletedTask; }
            Task IDevXHub.Notify(string flowId, string msg, object details) { return Task.CompletedTask; }
        }
    }
}
