using System;
using System.Collections.Generic;
using AutoMapper;
using Quali.Colony.Services.Common.OneToOneOrMany;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public class Client
        {
            public string ApiKey { get; set; }
            public string SessionId { get; set; }
            public string ConnectionId { get; set; }
        }

        public class User
        {
            public Guid UserId { get; set; }
            public string SessionId { get; set; }
            public string ApiKey { get; set; }
            public string ConnectionId { get; set; }
        }

        [One2One]
        public class ClientIsHereMsg
        {
            public string ApiKey { get; set; }
            public string SessionId { get; set; }
            public string ConnectionId { get; set; }
        }

        [One2Many]
        public class ClientIsGoneMsg
        {
            public string ApiKey { get; set; }
            public string SessionId { get; set; }
            public string ConnectionId { get; set; }
        }

        [One2Many]
        public class UserIsHereMsg
        {
            public Guid UserId { get; set; }
            public string SessionId { get; set; }
            public string ApiKey { get; set; }
            public string ConnectionId { get; set; }
        }

        [One2Many]
        public class PushMsg
        {
            public string ConnectionId { get; set; }
            public object Data { get; set; }
        }

        [One2One]
        public class NotifyMsg
        {
            public string Id { get; set; }
            public string FlowId { get; set; }
            public object Data { get; set; }
        }

        [One2Many]
        public class MapSessionMsg
        {
            public string MsgId { get; set; }
            public string TrackingId { get; set; }
            public string SessionId { get; set; }
        }

        [One2Many]
        public class MapFlowMsg
        {
            public string MsgId { get; set; }
            public string TrackingId { get; set; }
            public string FlowName { get; set; }
            public string FlowId { get; set; }
        }

        public class PocMessagesRequest
        {
            public string sandboxId { get; set; }
            public string terraformName { get; set; }
            public int delayOnStart { get; set; } = 0;
            public int delayOnMessage { get; set; } = 1;
        }

        static void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Client, ClientIsHereMsg>().ReverseMap();
                cfg.CreateMap<Client, ClientIsGoneMsg>().ReverseMap();
                cfg.CreateMap<Client, User>().ReverseMap();

                cfg.CreateMap<User, ClientIsHereMsg>().ReverseMap();
                cfg.CreateMap<User, UserIsHereMsg>().ReverseMap();

                cfg.CreateMap<NotifyMsg, PushMsg>().ReverseMap();
            });
        }

        private const object None = null;

        private static readonly Dictionary<string, object> _Models = new Dictionary<string, object>()
        {
            { "notification", new { msg = "", details = None } }
        };
    }
}
