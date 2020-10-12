using System;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class HistogramBucketFactoryTests
    {
        [Fact]
        public void ExponentialCountGreaterThanZero()
        {
            var factory = new HistogramBucketFactory();
            Assert.Throws<ArgumentOutOfRangeException>("count", () => factory.GetExponential(10, 10, 0));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => factory.GetExponential(10, 10, -1));
        }

        [Fact]
        public void ExponentialStartGreaterThanZero()
        {
            var factory = new HistogramBucketFactory();
            Assert.Throws<ArgumentOutOfRangeException>("start", () => factory.GetExponential(0, 10, 10));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => factory.GetExponential(-1, 10, 10));
        }

        [Fact]
        public void ExponentialExponentialGreaterThanOne()
        {
            var factory = new HistogramBucketFactory();
            Assert.Throws<ArgumentOutOfRangeException>("exponential", () => factory.GetExponential(10, 1, 10));
            Assert.Throws<ArgumentOutOfRangeException>("exponential", () => factory.GetExponential(10, 0, 10));
            Assert.Throws<ArgumentOutOfRangeException>("exponential", () => factory.GetExponential(10, -1, 10));
        }

        [Fact]
        public void ExponentialReturnsExponentialBuckets()
        {
            var factory = new HistogramBucketFactory();
            var buckets = factory.GetExponential(1, 2, 4);
            var expectedBuckets = new[] { 1D, 2D, 4D, 8D };
            Assert.Equal(expectedBuckets, buckets);
        }

        [Fact]
        public void GetExponentialUsesCache()
        {
            var factory = new HistogramBucketFactory();
            var expected = factory.GetExponential(1, 2, 4);
            var actual = factory.GetExponential(1, 2, 4);
            Assert.Same(expected, actual);
            Assert.Equal(new[] { 1D, 2D, 4D, 8D }, actual);
        }
    }
}
