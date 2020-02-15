using System;

namespace Chronosphere
{
    public class SystemClock : ISystemClock
    {
        private readonly IReadOnlyChronosphere _chronosphere;

        public SystemClock(IReadOnlyChronosphere chronosphere) => _chronosphere = chronosphere;

        public DateTimeOffset UtcNow => _chronosphere.UtcNow;

        public DateTimeOffset LocalNow => _chronosphere.LocalNow;
    }
}
