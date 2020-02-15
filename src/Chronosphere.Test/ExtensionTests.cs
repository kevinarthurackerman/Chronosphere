using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Chronosphere.Test
{
    [TestClass]
    public class ExtensionTests
    {
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
        public void ShouldGetRandomPastLocalDate()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            Assert.IsTrue(clock.RandomPastLocalDateTimeOffset() < clock.LocalNow);
        }

        [TestMethod]
        public void ShouldGetRandomPastUtcDate()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            Assert.IsTrue(clock.RandomPastUtcDateTimeOffset() < clock.UtcNow);
        }

        [TestMethod]
        public void ShouldGetRandomFutureLocalDate()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            Assert.IsTrue(clock.RandomFutureLocalDateTimeOffset() > clock.LocalNow);
        }

        [TestMethod]
        public void ShouldGetRandomFutureUtcDate()
        {
            using var scope = ServiceProvider.CreateScope();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();
            var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

            Assert.IsTrue(clock.RandomFutureUtcDateTimeOffset() > clock.UtcNow);
        }

        private IServiceProvider ServiceProvider = new ServiceCollection()
            .AddChronosphere()
            .BuildServiceProvider();
    }
}
