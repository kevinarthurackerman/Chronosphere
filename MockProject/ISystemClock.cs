using System;

namespace MockProject
{
    public interface ISystemClock
    {
        public DateTimeOffset UtcNow { get; }
    }
}
