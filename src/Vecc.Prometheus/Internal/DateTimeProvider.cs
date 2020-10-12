using System;
using System.Diagnostics.CodeAnalysis;

namespace Vecc.Prometheus.Internal
{
    [ExcludeFromCodeCoverage]
    public class DateTimeProvider : IDateTimeProvider
    {
        public static IDateTimeProvider Default = new DateTimeProvider();

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
    }
}
