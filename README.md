# Chronosphere

### One SystemClock to rule them all!
Chronosphere is the answer to how do I test time based features when Microsoft offers a diverse array of SystemClocks throughout their codebase? With Chronosphere you can control what time it is throughout the system in one place. Plus, you can control the time for each test, so you can fast forward and rewind and see if that auth token times out in 30 minutes without having to wait nearly that long.

### Getting Started
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

### Contributing
Want to contribute? Great!

### License
MIT
