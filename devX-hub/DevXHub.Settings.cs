using System;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public enum As
        {
            Api,            // can notify to others (default)
            Connector,      // clients connect here via SignalR (AgentConnector)
            Auth,           // authentication needs to access accounts DB (Accounts)
            Hub,            // relay happens here (Notifications)
            Gateway         // headers with sessions and flows (Gateway.Api)
        }

        public class Settings
        {
            public string BaseUrl { get; set; } = "/hub/devX";
            public string ApiKeyHeader { get; set; } = "devX-api-key";
            public string SessionIdHeader { get; set; } = "devX-session-id";
            public As StartAs { get; set; } = As.Api;
            public TimeSpan PollTimeout { get; set; } = 50.Seconds();

            public TimeSpan KeepAliveHeartbeat { get; set; } = 10.Seconds();
            public TimeSpan ClientIsDeadTimeout { get; set; } = 30.Seconds();

            public int IdRandomSuffixLength { get; set; } = 4;
        }
    }
}
