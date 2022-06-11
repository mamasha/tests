using System;

namespace Quali.Colony.Services.Common.devX_hub
{
    public class DevXHubException : Exception
    {
        public DevXHubException(string errMsg) : base(errMsg)
        {
        }
    }
}
