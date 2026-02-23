using System;
using Mamey.MicroMonolith.Abstractions.Time;

namespace Mamey.MicroMonolith.Infrastructure.Time;

public class UtcClock : IClock
{
    public DateTime CurrentDate() => DateTime.UtcNow;
}