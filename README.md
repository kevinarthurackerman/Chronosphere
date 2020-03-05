# Chronosphere

### One SystemClock to rule them all!
Chronosphere is the answer to how do I test time based features when Microsoft offers a diverse array of SystemClocks throughout their codebase? With Chronosphere you can control what time it is throughout the system in one place. Plus, you can control the time for each test, so you can fast forward and rewind and see if that auth token times out in 30 minutes without having to wait nearly that long.

### Getting Started
Control the current time for every ISystemClock in one place
```csharp
var serviceProvider = new ServiceCollection()
    .AddChronosphere()
    .BuildServiceProvider();
    
using var scope = serviceProvider.CreateScope();
var chrono = serviceProvider.GetRequiredService<IChronosphere>();
var systemClock = serviceProvider.GetRequiredService<Pick.A.Namespace.ISystemClock>();

Console.WriteLine(systemClock.UtcNow); // writes the current system time

chrono.Now = DateTimeOffset.MinValue;

Console.WriteLine(systemClock.UtcNow); // writes a time very near DateTimeOffset.MinValue
```
### Using In Tests
Helpful ISystemClock extensions make it easy to create and compare sample data where you are concerned by the relativity of moment that events occur more than the specific time.
```csharp
var sampleItem = new SampleItem { CreatedDateTime = _clock.RandomUtcPast() };
var sampleItemAction = new SampleItemAction { TimeStamp = _clock.RandomUtc(sampleItem.CreatedDateTime, TimePeriod.Present) }

Assert.IsTrue(sampleItem.CreatedDateTime < sampleItemAction.TimeSpamp);
Assert.IsTrue(sampleItemAction.TimeSpamp < _clock.UtcNow);
```

### Contributing
Want to contribute? Great!

### License
MIT
