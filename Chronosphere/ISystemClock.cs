using System;

namespace Chronosphere
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
