using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Chronosphere.Test
{
    [TestClass]
    public class ExtensionTests
    {
        private const int _testIterations = 10000;

        [TestMethod]
        public void ShouldFastForward()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            var now = clock.UtcNow;

            chrono.FastForward(TimeSpan.FromDays(1));

            var expected = now.AddDays(1);

            Assert.IsTrue(clock.UtcNow.IsApproximately(expected));
        }

        [TestMethod]
        public void ShouldRewind()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            var now = clock.UtcNow;

            chrono.Rewind(TimeSpan.FromDays(1));

            var expected = now.AddDays(-1);

            Assert.IsTrue(clock.UtcNow.IsApproximately(expected));
        }

        [TestMethod]
        public void ShouldSetCorrectOffset()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            Assert.IsTrue(Enumerable.Range(1, 12).All(x => clock.Random(TimeSpan.FromHours(x)).Offset == TimeSpan.FromHours(x)));
            Assert.IsTrue(Enumerable.Range(-12, 0).All(x => clock.Random(TimeSpan.FromHours(x)).Offset == TimeSpan.FromHours(x)));
        }

        [TestMethod]
        public void ShouldGetRandomFutureDate() => RunTest(x => x.RandomFuture(TimeSpan.FromHours(1)) > x.UtcNow.Add(TimeSpan.FromHours(1)));

        [TestMethod]
        public void ShouldGetRandomUtcFutureDate() => RunTest(x => x.RandomUtcFuture() > x.UtcNow);

        [TestMethod]
        public void ShouldGetRandomLocalFutureDate() => RunTest(x => x.RandomLocalFuture() > x.LocalNow);

        [TestMethod]
        public void ShouldGetRandomPastDate() => RunTest(x => x.RandomPast(TimeSpan.FromHours(1)) < x.UtcNow.Add(TimeSpan.FromHours(1)));

        [TestMethod]
        public void ShouldGetRandomUtcPastDate() => RunTest(x => x.RandomUtcPast() < x.UtcNow);

        [TestMethod]
        public void ShouldGetRandomPastLocalDate() => RunTest(x => x.RandomLocalPast() < x.LocalNow);

        [TestMethod]
        public void ShouldNotAllowToPast() =>
            Assert.ThrowsException<ArgumentException>(() => CreateClock().RandomUtc(TimePeriod.Present, TimePeriod.Past));

        [TestMethod]
        public void ShouldNotAllowFromFuture() =>
            Assert.ThrowsException<ArgumentException>(() => CreateClock().RandomUtc(TimePeriod.Future, TimePeriod.Present));

        [TestMethod]
        public void ShouldNotAllowMinGreaterThanMax() =>
            Assert.ThrowsException<ArgumentException>(() => {
                var clock = CreateClock();
                clock.RandomUtc(clock.UtcNow.AddHours(1).DateTime, clock.UtcNow.AddHours(-1).DateTime);
            });

        private void RunTest(Func<ISystemClock, bool> test)
        {
            var clock = CreateClock();
            
            Assert.IsTrue(Enumerable.Range(1, _testIterations).All(x => test(clock)));
        }

        private ISystemClock CreateClock()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            return clock;
        }

        private IServiceProvider ServiceProvider = new ServiceCollection()
            .AddChronosphere()
            .BuildServiceProvider();
    }
}
