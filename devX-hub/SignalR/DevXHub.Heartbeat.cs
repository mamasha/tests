using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;


namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        interface IHeartbeat
        {
            Task Start();
            void ClientIsHere(Client client);
            void ClientIsGone(Client client);
        }

        class Heartbeat : IHeartbeat
        {
            class Beat
            {
                public Client Client { get; set; }
                public DateTime LastBeat { get; set; }
                public DateTime LastMsg { get; set; }

                public int Count;
            }

            private readonly ILog _log;
            private readonly Settings _settings;
            private readonly IServiceProvider _di;
            private readonly Dictionary<string, Beat> _beats;

            public Heartbeat(ILog log,
                IOptions<Settings> settings,
                IServiceProvider di)
            {
                _log = log;
                _settings = settings.Value;
                _di = di;
                _beats = new Dictionary<string, Beat>();
            }

            private async Task sendMessages(DateTime now)
            {
                using var di = _di.CreateScope();

                var bus = di.ServiceProvider.GetService<IBus>();
                var beats = getAllBeats();

                if (beats.Length == 0)
                    return;

                _log.Info(new {now, clients = beats.Length});

                foreach (var beat in beats)
                {
                    var nextMsgAt = beat.LastMsg + _settings.KeepAliveHeartbeat;
                    var itsDead = beat.LastBeat + _settings.ClientIsDeadTimeout < now;

                    if (itsDead)
                    {
                        await bus.Call(beat.Client.To<ClientIsGoneMsg>());
                        continue;
                    }

                    if (nextMsgAt > now)
                        continue;

                    beat.LastMsg = now;
                    await bus.Call(beat.Client.To<ClientIsHereMsg>());
                }
            }

            private async Task loop()
            {
                var period = _settings.KeepAliveHeartbeat / 2;

                for (;;)
                {
                    await Task.Delay(period);

                    var now = DateTime.UtcNow;
                    await sendMessages(now);
                }
            }

            Task IHeartbeat.Start()
            {
                RunAsync(loop,
                    ex => _log.Warn(ex));

                return Task.CompletedTask;
            }

            private Beat getBeat(DateTime now, Client client)
            {
                lock (_beats)
                {
                    if (_beats.TryGetValue(client.SessionId, out var beat))
                        return beat;

                    beat = new Beat {
                        Client = client,
                        LastBeat = now,
                        LastMsg = now
                    };

                    _beats.Add(client.SessionId, beat);

                    return beat;
                }
            }

            private Beat removeBeat(Client client)
            {
                lock (_beats)
                {
                    if (!_beats.TryGetValue(client.SessionId, out var beat))
                        return null;

                    _beats.Remove(client.SessionId);

                    return beat;
                }
            }

            private Beat[] getAllBeats()
            {
                lock (_beats)
                {
                    return _beats.Values.ToArray();
                }
            }

            void IHeartbeat.ClientIsHere(Client client)
            {
                var now = DateTime.UtcNow;

                var beat = getBeat(now, client);

                beat.LastBeat = now;
                Interlocked.Increment(ref beat.Count);
            }

            void IHeartbeat.ClientIsGone(Client client)
            {
                var beat = removeBeat(client);

                if (beat != null)
                    _log.Info(beat);
            }
        }
    }
}
