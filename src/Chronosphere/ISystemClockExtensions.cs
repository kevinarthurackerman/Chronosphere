using System;

namespace Chronosphere
{
    public static class ISystemClockExtensions
    {
        public static DateTimeOffset RandomLocal(this ISystemClock systemClock) =>
            systemClock.Random(systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomLocal(this ISystemClock systemClock, DateTime min, DateTime max) =>
            systemClock.Random(min, max, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomLocal(this ISystemClock systemClock, DateTime min, TimePeriod max) =>
            systemClock.Random(min, max, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomLocal(this ISystemClock systemClock, TimePeriod min, DateTime max) =>
            systemClock.Random(min, max, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomLocal(this ISystemClock systemClock, TimePeriod min, TimePeriod max) =>
            systemClock.Random(min, max, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomLocalPast(this ISystemClock systemClock) =>
            systemClock.Random(TimePeriod.Past, TimePeriod.Present, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomLocalFuture(this ISystemClock systemClock) =>
            systemClock.Random(TimePeriod.Present, TimePeriod.Future, systemClock.LocalNow.Offset);

        public static DateTimeOffset RandomUtc(this ISystemClock systemClock) =>
            systemClock.Random(TimeSpan.Zero);

        public static DateTimeOffset RandomUtc(this ISystemClock systemClock, DateTime min, DateTime max) =>
            systemClock.Random(min, max, TimeSpan.Zero);

        public static DateTimeOffset RandomUtc(this ISystemClock systemClock, DateTime min, TimePeriod max) =>
            systemClock.Random(min, max, TimeSpan.Zero);

        public static DateTimeOffset RandomUtc(this ISystemClock systemClock, TimePeriod min, DateTime max) =>
            systemClock.Random(min, max, TimeSpan.Zero);

        public static DateTimeOffset RandomUtc(this ISystemClock systemClock, TimePeriod min, TimePeriod max) =>
               systemClock.Random(min, max, TimeSpan.Zero);

        public static DateTimeOffset RandomUtcPast(this ISystemClock systemClock) =>
            systemClock.Random(TimePeriod.Past, TimePeriod.Present, TimeSpan.Zero);

        public static DateTimeOffset RandomUtcFuture(this ISystemClock systemClock) =>
            systemClock.Random(TimePeriod.Present, TimePeriod.Future, TimeSpan.Zero);

        public static DateTimeOffset Random(this ISystemClock systemClock, TimeSpan offset)
        {
            var now = systemClock.UtcNow.DateTime;
            return RandomDateTimeOffset(now - ChronosphereConstants.MaxOffset, now + ChronosphereConstants.MaxOffset, offset);
        }

        public static DateTimeOffset Random(this ISystemClock systemClock, DateTime min, DateTime max, TimeSpan offset) =>
            RandomDateTimeOffset(min, max, offset);

        public static DateTimeOffset Random(this ISystemClock systemClock, DateTime min, TimePeriod max, TimeSpan offset) =>
            RandomDateTimeOffset(min, CalculateMax(systemClock.UtcNow.DateTime, max), offset);

        public static DateTimeOffset Random(this ISystemClock systemClock, TimePeriod min, DateTime max, TimeSpan offset) =>
            RandomDateTimeOffset(CalculateMin(systemClock.UtcNow.DateTime, min), max, offset);

        public static DateTimeOffset Random(this ISystemClock systemClock, TimePeriod min, TimePeriod max, TimeSpan offset)
        {
            var now = systemClock.UtcNow.DateTime;
            return RandomDateTimeOffset(CalculateMin(now, min), CalculateMax(now, max), offset);
        }

        public static DateTimeOffset RandomPast(this ISystemClock systemClock, TimeSpan offset) =>
            systemClock.Random(TimePeriod.Past, TimePeriod.Present, offset);

        public static DateTimeOffset RandomFuture(this ISystemClock systemClock, TimeSpan offset) =>
            systemClock.Random(TimePeriod.Present, TimePeriod.Future, offset);

        [Obsolete]
        public static DateTimeOffset RandomLocalDateTimeOffset(this ISystemClock systemClock, DateTimeOffset min, DateTimeOffset max) =>
            RandomDateTimeOffset(min.DateTime, max.DateTime, systemClock.LocalNow.Offset);

        [Obsolete]
        public static DateTimeOffset RandomPastLocalDateTimeOffset(this ISystemClock systemClock) =>
            RandomPastLocalDateTimeOffset(systemClock, ChronosphereConstants.OffsetTolerance);

        [Obsolete]
        public static DateTimeOffset RandomPastLocalDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance)
        {
            var now = systemClock.LocalNow.DateTime;
            return RandomDateTimeOffset(now - ChronosphereConstants.MaxOffset, now - tolerance, systemClock.LocalNow.Offset);

        }

        [Obsolete]
        public static DateTimeOffset RandomPastUtcDateTimeOffset(this ISystemClock systemClock) =>
            RandomPastUtcDateTimeOffset(systemClock, ChronosphereConstants.OffsetTolerance);

        [Obsolete]
        public static DateTimeOffset RandomPastUtcDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance)
        {
            var now = systemClock.UtcNow.DateTime;
            return RandomDateTimeOffset(now - ChronosphereConstants.MaxOffset, now - tolerance, TimeSpan.Zero);
        }

        [Obsolete]
        public static DateTimeOffset RandomFutureLocalDateTimeOffset(this ISystemClock systemClock) =>
            RandomFutureLocalDateTimeOffset(systemClock, ChronosphereConstants.OffsetTolerance);

        [Obsolete]
        public static DateTimeOffset RandomFutureLocalDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance)
        {
            var now = systemClock.LocalNow.DateTime;
            return RandomDateTimeOffset(now + tolerance, now + ChronosphereConstants.MaxOffset, systemClock.LocalNow.Offset);

        }

        [Obsolete]
        public static DateTimeOffset RandomFutureUtcDateTimeOffset(this ISystemClock systemClock) =>
            RandomFutureUtcDateTimeOffset(systemClock, ChronosphereConstants.OffsetTolerance);

        [Obsolete]
        public static DateTimeOffset RandomFutureUtcDateTimeOffset(this ISystemClock systemClock, TimeSpan tolerance)
        {
            var now = systemClock.UtcNow.DateTime;
            return RandomDateTimeOffset(now + tolerance, now + ChronosphereConstants.MaxOffset, TimeSpan.Zero);
        }

        private static DateTime CalculateMin(DateTime now, TimePeriod min) =>
            min switch
            {
                TimePeriod.Present => now + ChronosphereConstants.OffsetTolerance,
                TimePeriod.Past => now - ChronosphereConstants.MaxOffset,
                TimePeriod.Future => throw new ArgumentException($"Cannot set a {nameof(min)} limit of {nameof(TimePeriod.Future)}", nameof(min)),
                _ => throw new ArgumentException($"Invalid value {(int)min} selected for {nameof(min)}", nameof(min)),
            };

        private static DateTime CalculateMax(DateTime now, TimePeriod max) =>
            max switch
            {
                TimePeriod.Present => now - ChronosphereConstants.OffsetTolerance,
                TimePeriod.Past => throw new ArgumentException($"Cannot set a {nameof(max)} limit of {nameof(TimePeriod.Past)}", nameof(max)),
                TimePeriod.Future => now + ChronosphereConstants.MaxOffset,
                _ => throw new ArgumentException($"Invalid value {(int)max} selected for {nameof(max)}", nameof(max)),
            };

        private static DateTimeOffset RandomDateTimeOffset(DateTime minimum, DateTime maximum, TimeSpan offset)
        {
            if (minimum >= maximum) throw new ArgumentException($"{nameof(minimum)} cannot be greater than {nameof(maximum)}");

            var degreeOfNormalization = ChronosphereConstants.DegreeOfNormalization;

            var randomAmount = 0.0;

            for (var i = 0; i < degreeOfNormalization; i++) randomAmount += Internal.Random.NextDouble();

            randomAmount /= degreeOfNormalization;

            return new DateTimeOffset((long)(randomAmount * (maximum.Ticks - minimum.Ticks) + minimum.Ticks), offset);
        }
    }
}
