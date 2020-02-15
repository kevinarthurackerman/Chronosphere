using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chronosphere.Test
{
    [TestClass]
    public class DependencyInjectionTests
    {
        static DependencyInjectionTests()
        {
            // ensure that we have referenced the assemblies
            var authClock = typeof(Microsoft.AspNetCore.Authentication.ISystemClock);
            var internalClock = typeof(Microsoft.Extensions.Internal.ISystemClock);
            var kestrelClock = typeof(Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure.ISystemClock);
            var customProject = typeof(MockProject.MarkerType);
        }

        [TestMethod]
        public void ShouldRegisterSystemClocks()
        {
            var expected = new[]
            {
                typeof(ISystemClock),
                typeof(Microsoft.AspNetCore.Authentication.ISystemClock),
                typeof(Microsoft.Extensions.Internal.ISystemClock),
                typeof(Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure.ISystemClock),
                typeof(MockProject.ISystemClock)
            };

            var actual = GetSystemClockTypes().ToArray();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ShouldInstantiateClocks()
        {
            using var scope = new ServiceCollection()
                .AddChronosphere()
                .BuildServiceProvider()
                .CreateScope();

            var clockTypes = GetSystemClockTypes();

            foreach(var clockType in clockTypes)
                try
                {
                    scope.ServiceProvider.GetRequiredService(clockType);
                }
                catch(Exception e)
                {
                    Assert.Fail($"Failed to instantiate SystemClock of type '{clockType.FullName}'. Threw an exception of type '{e.GetType().FullName}' with message '{e.Message}'");
                }
        }

        [TestMethod]
        public void ShouldSetTime()
        {
            using var scope = new ServiceCollection()
                .AddChronosphere()
                .BuildServiceProvider()
                .CreateScope();

            var clockTypes = GetSystemClockTypes();

            var chrono = scope.ServiceProvider.GetRequiredService<IChronosphere>();

            var offset = TimeSpan.FromHours(6);
            chrono.Now = new DateTimeOffset(2020, 1, 1, 0, 0, 0, offset);

            foreach (var clockType in clockTypes)
            {
                var clock = (ISystemClock)scope.ServiceProvider.GetRequiredService(clockType);

                Assert.IsTrue(clock.UtcNow.ToOffset(offset).IsApproximately(chrono.Now));
            }

            chrono.Now = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

            foreach (var clockType in clockTypes)
            {
                var clock = (ISystemClock)scope.ServiceProvider.GetRequiredService(clockType);

                Assert.IsTrue(clock.UtcNow.IsApproximately(chrono.Now));
            }
        }

        private IEnumerable<Type> GetSystemClockTypes() => new ServiceCollection()
                .AddChronosphere()
                .Where(x => typeof(ISystemClock).IsAssignableFrom(x.ImplementationType))
                .Select(x => x.ServiceType)
                .AsEnumerable();
    }
}
