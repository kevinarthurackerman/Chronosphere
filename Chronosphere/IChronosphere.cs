using System;

namespace Chronosphere
{
    public interface IChronosphere : IReadOnlyChronosphere
    {
        new DateTimeOffset Now { get; set; }
    }
}
