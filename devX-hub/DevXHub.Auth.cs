using System;
using System.Threading.Tasks;
using Dapper;
using Quali.Colony.DataAccess;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        interface IAuth
        {
            Task ClientIsHere(ClientIsHereMsg msg);
        }

        class PostgresqlSettings : Quali.Colony.DataAccess.PostgresqlSettings
        {
        }

        class Auth : IAuth
        {
            private readonly ILog _log;
            private readonly IBus _bus;
            private readonly IRelay _relay;
            private readonly IDbConnectionFactory<PostgresqlSettings> _connectionFactory;

            public Auth(
                ILog log,
                IBus bus,
                IRelay relay,
                IDbConnectionFactory<PostgresqlSettings> connectionFactory)
            {
                _log = log;
                _bus = bus;
                _relay = relay;
                _connectionFactory = connectionFactory;
            }

            private async Task<Guid> queryUserId(string apiKey)
            {
                using var cn = _connectionFactory.GetOrOpenConnection();

                var sql = @"select user_id from user_token where token=@apiKey";

                var userId = await cn.Connection.QueryFirstOrDefaultAsync<Guid>(sql, new { apiKey });

                _log.Info(new { sql, userId });

                return userId;
            }

            async Task IAuth.ClientIsHere(ClientIsHereMsg msg)
            {
                var client = msg.To<Client>();

                var userId = await queryUserId(client.ApiKey);

                if (userId == Guid.Empty)
                {
                    _log.Warn("ApiKey is not found", client);
                    return;
                }

                var user = client.To<User>(
                    u => u.UserId = userId);

                _log.Info("Client is authenticated", user);

                await _bus.Call(_relay.UserIsHere, user.To<UserIsHereMsg>());
            }

            public class Null : IAuth
            {
                Task IAuth.ClientIsHere(ClientIsHereMsg msg) { return Task.CompletedTask; }
            }
        }
    }
}
