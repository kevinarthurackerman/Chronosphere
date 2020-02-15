using System;

namespace Chronosphere.Test
{
    public static class TestHelpers
    {
        private static TimeSpan _tolerance = TimeSpan.FromMinutes(1);

        public static bool IsApproximately(this DateTimeOffset dateTimeOffset, DateTimeOffset otherDateTimeOffset)
        {
            var max = dateTimeOffset + _tolerance;
            var min = dateTimeOffset - _tolerance;
            var otherDateTimeOffsetUtc = otherDateTimeOffset.UtcDateTime;

            return otherDateTimeOffsetUtc <= max && otherDateTimeOffsetUtc >= min;
        }
    }
}
