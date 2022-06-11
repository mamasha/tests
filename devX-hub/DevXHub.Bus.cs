using MassTransit;
using Microsoft.Extensions.Logging;
using Quali.Colony.Redis;
using Quali.Colony.Services.Common.OneToOneOrMany;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public interface IBus : IOneToOneOrManyBus
        {

        }

        class Bus : OneToOneOrManyBus, IBus
        {
            public Bus(
                ILogger<OneToOneOrManyBus> logger,
                IPublishEndpoint one2One,
                IRedisPublisher one2Many)
                : base(logger, one2One, one2Many)
            {
            }
        }
    }
}
