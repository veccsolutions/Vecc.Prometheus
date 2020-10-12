using System.Threading;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests
{
    public class GaugeExtensionsTests
    {
        [Fact]
        public void TrackCountIncrementsAndDecrements()
        {
            var gauge = new Gauge(null, null);

            using (var tracker = GaugeExtensions.TrackCount(gauge))
            {
                //should be 1
                Assert.Equal(1, gauge.Value);
            }

            Assert.Equal(0, gauge.Value);
        }

        [Fact]
        public void TrackTimeRecordsTimeTaken()
        {
            var gauge = new Gauge(null, null);

            using (var tracker = GaugeExtensions.TrackTime(gauge))
            {
                Thread.Sleep(100);
            }

            //fudge factor to allow for clock skew and timings.
            Assert.True(gauge.Value > .09);
            Assert.True(gauge.Value < .2);
        }
    }
}
