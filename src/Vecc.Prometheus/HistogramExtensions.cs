using System;
using Vecc.Prometheus.Internal;

namespace Vecc.Prometheus
{
    public static class HistogramExtensions
    {
        public static IDisposable TrackTime(this IHistogram histogram)
        {
            var now = DateTimeProvider.Default.UtcNow;

            return new CallbackDisposable(() => histogram.Add((DateTimeProvider.Default.UtcNow - now).TotalSeconds));
        }
    }
}
