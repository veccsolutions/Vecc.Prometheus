using System;

namespace Vecc.Prometheus.Internal
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTimeOffset OffsetNow { get; }
    }
}
