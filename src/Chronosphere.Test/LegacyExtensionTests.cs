using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Chronosphere.Test
{
    [TestClass]
    public class LegacyExtensionTests
    {
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
