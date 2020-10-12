using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class HistogramTests
    {
        [Fact]
        public async Task ScrapeReturnsCorrectWithIdentifier()
        {
            var histogram = new Histogram("test", "test", new[] { 1D, 2D, 4D });
            histogram.Add(3);
            var stringWriter = new StringWriter();
            await histogram.ScrapeAsync(stringWriter);
            Assert.Equal(
                "test_bucket{le=\"1\",test} 0\n" +
                "test_bucket{le=\"2\",test} 0\n" +
                "test_bucket{le=\"4\",test} 1\n" +
                "test_bucket{le=\"+Inf\",test} 1\n" +
                "test_sum{test} 3\n" +
                "test_count{test} 1\n", stringWriter.ToString());
        }

        [Fact]
        public async Task ScrapeReturnsCorrectWithoutIdentifier()
        {
            //TODO: verify this is correct with the empty , in the buckets and {} in the sum/count
            var histogram = new Histogram(null, "test", new[] { 1D, 2D, 4D });
            histogram.Add(3);
            var stringWriter = new StringWriter();
            await histogram.ScrapeAsync(stringWriter);
            Assert.Equal(
                "test_bucket{le=\"1\",} 0\n" +
                "test_bucket{le=\"2\",} 0\n" +
                "test_bucket{le=\"4\",} 1\n" +
                "test_bucket{le=\"+Inf\",} 1\n" +
                "test_sum{} 3\n" +
                "test_count{} 1\n", stringWriter.ToString());

            histogram = new Histogram(string.Empty, "test", new[] { 1D, 2D, 4D });
            histogram.Add(3);
            stringWriter = new StringWriter();
            await histogram.ScrapeAsync(stringWriter);
            Assert.Equal(
                "test_bucket{le=\"1\",} 0\n" +
                "test_bucket{le=\"2\",} 0\n" +
                "test_bucket{le=\"4\",} 1\n" +
                "test_bucket{le=\"+Inf\",} 1\n" +
                "test_sum{} 3\n" +
                "test_count{} 1\n", stringWriter.ToString());
        }

        [Fact]
        public async Task CallbacksAreExecuted()
        {
            var called = false;
            var histogram = new Histogram("test", "test", new[] { 1D, 2D, 4D });
            histogram.AddBeforeScrapeCallback((metric) =>
            {
                Assert.Same(histogram, metric);
                called = true;
                return Task.CompletedTask;
            });
            await histogram.ScrapeAsync(new StringWriter());
            Assert.True(called);
        }

        [Fact]
        public void GetBucketsReturnsBucketsAndCounts()
        {
            var histogram = new Histogram(null, "test", new[] { 1D, 2D, 4D });
            histogram.Add(3);
            Assert.Equal((new[] {
                new KeyValuePair<double, double>(1D, 0),
                new KeyValuePair<double, double>(2D, 0),
                new KeyValuePair<double, double>(4D, 1) }).ToDictionary(x => x.Key, x => x.Value),
                histogram.GetBuckets());
        }
    }
}
