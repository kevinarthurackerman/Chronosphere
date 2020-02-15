using System;

namespace Chronosphere
{
    public static class ISystemClockExtensions
    {
        private static TimeSpan _defaultOffsetTolerance = TimeSpan.FromMinutes(10);

        public static DateTimeOffset RandomLocalDateTimeOffset(this ISystemClock systemClock) =>
            RandomDateTimeOffset(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomUtcDateTimeOffset(this ISystemClock systemClock) =>
            RandomDateTimeOffset(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

        public static DateTimeOffset RandomPastLocalDateTimeOffset(this ISystemClock systemClock) =>
            RandomPastLocalDateTimeOffset(systemClock, _defaultOffsetTolerance);

        public static DateTimeOffset RandomPastLocalDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance)
        {
            var now = systemClock.LocalNow;
            return RandomDateTimeOffset(DateTimeOffset.MinValue, now - tolerance, now.Offset);

        }

        public static DateTimeOffset RandomPastUtcDateTimeOffset(this ISystemClock systemClock) =>
            RandomPastUtcDateTimeOffset(systemClock, _defaultOffsetTolerance);

        public static DateTimeOffset RandomPastUtcDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance) =>
            RandomDateTimeOffset(DateTimeOffset.MinValue, systemClock.UtcNow - tolerance);

        public static DateTimeOffset RandomFutureLocalDateTimeOffset(this ISystemClock systemClock) =>
            RandomFutureLocalDateTimeOffset(systemClock, _defaultOffsetTolerance);

        public static DateTimeOffset RandomFutureLocalDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance)
        {
            var now = systemClock.LocalNow;
            return RandomDateTimeOffset(now + tolerance, DateTimeOffset.MaxValue, now.Offset);

        }

        public static DateTimeOffset RandomFutureUtcDateTimeOffset(this ISystemClock systemClock) =>
            RandomFutureUtcDateTimeOffset(systemClock, _defaultOffsetTolerance);

        public static DateTimeOffset RandomFutureUtcDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance) =>
            RandomDateTimeOffset(systemClock.UtcNow + tolerance, DateTimeOffset.MaxValue);

        private static DateTimeOffset RandomDateTimeOffset(DateTimeOffset minimum, DateTimeOffset maximum, TimeSpan offset = default) =>
            new DateTimeOffset((long)(Internal.Random.NextDouble() * (maximum.Ticks - minimum.Ticks) + minimum.Ticks), offset);
    }
}
