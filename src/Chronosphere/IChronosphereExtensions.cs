using System;

namespace Chronosphere
{
    public static class IChronosphereExtensions
    {
        public static void FastForward(this IChronosphere chronosphere, TimeSpan time) => chronosphere.Now = chronosphere.Now + time;

        public static void Rewind(this IChronosphere chronosphere, TimeSpan time) => chronosphere.Now = chronosphere.Now - time;
    }
}
