using System.Collections.Generic;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        interface IRelayRepo
        {
            void AddUser(string sessionId, User user);
            void RemoveUser(string sessionId);
            bool TryGetUser(string sessionId, out User user);

            void AddSession(string trackingId, string sessionId);
            bool TryGetSession(string trackingId, out string sessionId);

            void AddFlow(string flowId, string trackingId);
            bool TryGetFlow(string flowId, out string trackingId);
        }

        class RelayRepo : IRelayRepo
        {
            private readonly Dictionary<string, User> _users;
            private readonly Dictionary<string, string> _sessions;

            public RelayRepo()
            {
                _users = new Dictionary<string, User>();
                _sessions = new Dictionary<string, string>();
            }

            void IRelayRepo.AddUser(string sessionId, User user)
            {
                lock (_users)
                {
                    _users[sessionId] = user;
                }
            }

            void IRelayRepo.RemoveUser(string sessionId)
            {
                lock (_users)
                {
                    _users.Remove(sessionId);
                }
            }

            bool IRelayRepo.TryGetUser(string sessionId, out User user)
            {
                lock (_users)
                {
                    return _users.TryGetValue(sessionId, out user);
                }
            }

            void IRelayRepo.AddSession(string trackingId, string sessionId)
            {
                lock (_sessions)
                {
                    _sessions[trackingId] = sessionId;
                }
            }

            bool IRelayRepo.TryGetSession(string trackingId, out string sessionId)
            {
                lock (_sessions)
                {
                    return _sessions.TryGetValue(trackingId, out sessionId);
                }
            }

            void IRelayRepo.AddFlow(string flowId, string trackingId)
            {
                lock (_sessions)
                {
                    _sessions[flowId] = trackingId;
                }
            }

            bool IRelayRepo.TryGetFlow(string flowId, out string trackingId)
            {
                lock (_sessions)
                {
                    return _sessions.TryGetValue(flowId, out trackingId);
                }
            }

            public class Null : IRelayRepo
            {
                void IRelayRepo.AddUser(string sessionId, User user) { }
                void IRelayRepo.RemoveUser(string sessionId) { }
                bool IRelayRepo.TryGetUser(string sessionId, out User user) { user = null; return false; }
                void IRelayRepo.AddSession(string trackingId, string sessionId) { }
                bool IRelayRepo.TryGetSession(string trackingId, out string sessionId) { sessionId = null; return false; }
                void IRelayRepo.AddFlow(string flowId, string trackingId) { }
                bool IRelayRepo.TryGetFlow(string flowId, out string trackingId)  { trackingId = null; return false; }
            }
        }
    }
}
