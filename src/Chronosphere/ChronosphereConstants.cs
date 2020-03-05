using System;

namespace Chronosphere
{
    public static class ChronosphereConstants
    {
        public static TimeSpan OffsetTolerance = TimeSpan.FromMinutes(2);

        public static TimeSpan MaxOffset = TimeSpan.FromDays(365 * 200);

        public static int DegreeOfNormalization = 3;
    }
}
