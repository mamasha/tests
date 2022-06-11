using System.Collections.Generic;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        interface IIoRepo
        {
            void AddConnection(string connectionId);
            void RemoveConnection(string connectionId);
            bool ItsMyConnection(string connectionId);
        }

        class IoRepo : IIoRepo
        {
            private readonly HashSet<string> _connections;

            public IoRepo()
            {
                _connections = new HashSet<string>();
            }

            void IIoRepo.AddConnection(string connectionId)
            {
                lock (_connections)
                {
                    _connections.Add(connectionId);
                }
            }

            void IIoRepo.RemoveConnection(string connectionId)
            {
                lock (_connections)
                {
                    _connections.Remove(connectionId);
                }
            }

            bool IIoRepo.ItsMyConnection(string connectionId)
            {
                lock (_connections)
                {
                    return _connections.Contains(connectionId);
                }
            }

            public class Null : IIoRepo
            {
                void IIoRepo.AddConnection(string connectionId) { }
                void IIoRepo.RemoveConnection(string connectionId) { }
                bool IIoRepo.ItsMyConnection(string connectionId) { return false; }
            }
        }
    }
}
