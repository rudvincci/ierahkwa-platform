using Mamey.Time;

namespace Mamey.Microservice.Infrastructure.Time
{
    public class UtcClock : IClock
    {
        public DateTime CurrentDate() => DateTime.UtcNow;
    }
}

