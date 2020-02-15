using System;

namespace Chronosphere
{
    public class Chronosphere : IChronosphere
    {
        private TimeSpan _offsetFromSystemTime;
        private TimeSpan _timezoneOffset;

        public Chronosphere()
        {
            _offsetFromSystemTime = TimeSpan.Zero;
            _timezoneOffset = TimeSpan.Zero;
        }

        public Chronosphere(DateTime now)
        {
            _offsetFromSystemTime = now - DateTime.UtcNow;
            _timezoneOffset = TimeSpan.Zero;
        }

        public Chronosphere(DateTimeOffset now)
        {
            _offsetFromSystemTime = now.UtcDateTime - DateTime.UtcNow;
            _timezoneOffset = now.Offset;
        }

        public DateTimeOffset Now
        {
            get => new DateTimeOffset(DateTime.UtcNow + _offsetFromSystemTime, _timezoneOffset);
            set {
                _offsetFromSystemTime = value.UtcDateTime - DateTime.UtcNow;
                _timezoneOffset = value.Offset;
            }
        }
    }
}
