using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        class IoIn : Hub
        {
            private readonly Settings _settings;
            private readonly ILog _log;
            private readonly IAuth _auth;
            private readonly IRelay _relay;
            private readonly IBus _bus;
            private readonly IHeartbeat _heartbeat;
            private readonly IIoRepo _repo;

            public IoIn(
                IOptions<Settings> settings,
                ILog log,
                IAuth auth,
                IRelay relay,
                IBus bus,
                IHeartbeat heartbeat,
                IIoRepo repo)
            {
                _settings = settings.Value;
                _log = log;
                _auth = auth;
                _relay = relay;
                _bus = bus;
                _heartbeat = heartbeat;
                _repo = repo;
            }

            private Client getClient()
            {
                var headers = Context.GetHttpContext().Request.Headers;

                var allRequiredHeadersAreThere =
                    headers.TryGetValue(_settings.ApiKeyHeader, out var apiKeys);

                allRequiredHeadersAreThere &=
                    headers.TryGetValue(_settings.SessionIdHeader, out var sessionIds);

                var apiKey = apiKeys.First();
                var sessionId = sessionIds.First();
                var connectionId = Context.ConnectionId;

                allRequiredHeadersAreThere &=
                    !string.IsNullOrWhiteSpace(apiKey) &&
                    !string.IsNullOrWhiteSpace(sessionId);

                if (!allRequiredHeadersAreThere)
                    throw new DevXHubException("Malformed headers in connection");

                return new Client {
                    ApiKey = apiKey,
                    SessionId = sessionId,
                    ConnectionId = connectionId
                };
            }

            private async Task clientIsHere(Client client)
            {
                _repo.AddConnection(client.ConnectionId);
                await _bus.Call(_auth.ClientIsHere, client.To<ClientIsHereMsg>());
            }

            private async Task clientIsGone(Client client)
            {
                _repo.RemoveConnection(client.ConnectionId);
                await _bus.Call(_relay.ClientIsGone, client.To<ClientIsGoneMsg>());
            }

            private void startHeartbeats(Client client)
            {
                Context.Features.Get<IConnectionHeartbeatFeature>()
                    .OnHeartbeat(me => { _heartbeat.ClientIsHere((Client) me); }, client);
            }

            private async Task onConnectedAsync()
            {
                var client = getClient();
                _log.Info(client);

                await clientIsHere(client);

                startHeartbeats(client);
            }

            private async Task onDisconnectedAsync()
            {
                var client = getClient();
                _log.Info(client);

                await clientIsGone(client);
            }

            public override Task OnConnectedAsync() => Try.Catch.Rethrow(onConnectedAsync,
                ex => _log.Warn(ex));

            public override Task OnDisconnectedAsync(Exception err) => Try.Catch.Handle(onDisconnectedAsync,
                ex => _log.Warn(ex));
        }
    }
}
