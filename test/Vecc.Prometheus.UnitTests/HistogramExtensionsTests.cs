using System.Threading;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests
{
    public class HistogramExtensionsTests
    {
        [Fact]
        public void TrackTimeAddsSeconds()
        {
            var hist = new Histogram(null, null, new[] { .25D, 1D });

            using (var tracker = HistogramExtensions.TrackTime(hist))
            {
                Thread.Sleep(500);
            }

            Assert.Equal(1, hist.GetBuckets()[1D]);
        }
    }
}
