using System;

namespace Chronosphere
{
    public class Chronosphere : IChronosphere
    {
        private TimeSpan _offsetFromSystemTime;

        private TimeSpan _timeZoneOffset;

        public Chronosphere()
        {
            _offsetFromSystemTime = TimeSpan.Zero;
            _timeZoneOffset = TimeSpan.Zero;
        }

        public Chronosphere(DateTime now)
        {
            _offsetFromSystemTime = now - DateTime.UtcNow;
            _timeZoneOffset = TimeSpan.Zero;
        }

        public Chronosphere(DateTimeOffset now)
        {
            _offsetFromSystemTime = now.UtcDateTime - DateTime.UtcNow;
            _timeZoneOffset = now.Offset;
        }

        public DateTimeOffset Now
        {
            get => new DateTimeOffset(DateTime.UtcNow.Ticks + _offsetFromSystemTime.Ticks, TimeSpan.Zero).ToOffset(_timeZoneOffset);
            set {
                _offsetFromSystemTime = value.UtcDateTime - DateTime.UtcNow;
                _timeZoneOffset = value.Offset;
            }
        }

        public DateTimeOffset UtcNow => new DateTimeOffset(DateTime.UtcNow + _offsetFromSystemTime, TimeSpan.Zero);

        public DateTimeOffset LocalNow => new DateTimeOffset(DateTime.UtcNow + _offsetFromSystemTime).ToOffset(_timeZoneOffset);
    }
}
