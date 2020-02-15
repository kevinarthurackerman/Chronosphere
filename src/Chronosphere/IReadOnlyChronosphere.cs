using System;

namespace Chronosphere
{
    public interface IReadOnlyChronosphere: ISystemClock
    {
        DateTimeOffset Now { get; }
    }
}
