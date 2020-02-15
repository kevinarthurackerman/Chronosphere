namespace Chronosphere
{
    public static class IReadOnlyChronosphereExtensions
    {
        public static ISystemClock CreateClock(this IReadOnlyChronosphere chronosphere) => new SystemClock(chronosphere);
    }
}
