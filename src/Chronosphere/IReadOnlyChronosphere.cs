using System;

namespace Chronosphere
{
    public interface IReadOnlyChronosphere
    {
        DateTimeOffset Now { get; }
    }
}
