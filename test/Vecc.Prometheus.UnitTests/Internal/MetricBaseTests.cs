using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class MetricBaseTests
    {
        [Fact]
        public void AddDoubleRollsOver()
        {
            var metric = new MetricBaseClass();
            Assert.Equal(101D, metric.AddDouble(100D, 1D));
            Assert.Equal(75D, metric.AddDouble(double.MaxValue, 75D));
        }

        private class MetricBaseClass : MetricBase
        {
        }
    }
}
