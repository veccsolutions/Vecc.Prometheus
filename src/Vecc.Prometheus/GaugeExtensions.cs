using System;
using Vecc.Prometheus.Internal;

namespace Vecc.Prometheus
{
    public static class GaugeExtensions
    {
        public static IDisposable TrackCount(this IGauge gauge)
        {
            gauge.Add(1);

            return new CallbackDisposable(() => gauge.Add(-1));
        }

        public static IDisposable TrackTime(this IGauge gauge)
        {
            var now = DateTimeProvider.Default.UtcNow;

            return new CallbackDisposable(() => gauge.Set((DateTimeProvider.Default.UtcNow - now).TotalSeconds));
        }
    }
}
