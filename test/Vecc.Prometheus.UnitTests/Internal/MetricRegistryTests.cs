using System;
using System.Threading.Tasks;
using Moq;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class MetricRegistryTests
    {
        [Fact]
        public async Task CallbacksAreExecuted()
        {
            var called = false;
            var metricFactory = new MetricFactory();
            var bucketFactory = new HistogramBucketFactory();
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.SetupGet(x => x.OffsetNow).Returns(new DateTimeOffset(2020, 1, 1, 1, 1, 1, new TimeSpan(1, 0, 0)));

            var registry = new MetricRegistry(dateTimeProvider.Object, bucketFactory, metricFactory);

            registry.AddBeforeScrapeCallback((r) =>
            {
                Assert.Same(registry, r);
                called = true;
                return Task.CompletedTask;
            });

            var scrape = await registry.ScrapeAsync();
            Assert.Equal("# Scraped at 1/1/2020 1:01:01 AM +01:00\n", scrape);
            Assert.True(called);
        }

        [Fact]
        public async Task MetricsAreScraped()
        {
            var metricFactory = new MetricFactory();
            var bucketFactory = new HistogramBucketFactory();
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.SetupGet(x => x.OffsetNow).Returns(new DateTimeOffset(2020, 1, 1, 1, 1, 1, new TimeSpan(1, 0, 0)));

            var registry = new MetricRegistry(dateTimeProvider.Object, bucketFactory, metricFactory);

            var counter = registry.GetCounter("counter", "Help text");
            var gauge = registry.GetGauge("gauge");
            var histogram = registry.GetHistogram("histogram", new[] { .5D, 1D });
            var exponentialHistogram = registry.GetHistogramExponential("histogramExponential", 1, 2, 4);
            var customCounter = registry.Factory.GetCounter("counter1", null, null);
            var customGauge = registry.Factory.GetGauge("gauge1", null, null);
            var customHistogram = registry.Factory.GetHistogram("histogram1", null, null, 1D, 2D, 4D);

            registry.RegisterCounter(customCounter.Name, customCounter);
            registry.RegisterGauge(customGauge.Name, customGauge);
            registry.RegisterHistogram(customHistogram.Name, customHistogram);

            counter.Get().Increment();
            gauge.Get().Add(1);
            histogram.Get().Add(.75D);
            exponentialHistogram.Get().Add(3);
            customCounter.Get().Increment();
            customGauge.Get().Add(1);
            customHistogram.Get().Add(.75D);

            var scrape = await registry.ScrapeAsync();

            Assert.Equal("# Scraped at 1/1/2020 1:01:01 AM +01:00\n" +
                "# HELP Help text\n" +
                "# TYPE counter counter\n" +
                "counter 1\n" +
                "\n" +
                "# TYPE counter1 counter\n" +
                "counter1 1\n" +
                "\n" +
                "# TYPE gauge gauge\n" +
                "gauge 1\n" +
                "\n" +
                "# TYPE gauge1 gauge\n" +
                "gauge1 1\n" +
                "\n" +
                "# TYPE histogram histogram\n" +
                "histogram_bucket{le=\"0.5\",} 0\n" +
                "histogram_bucket{le=\"1\",} 1\n" +
                "histogram_bucket{le=\"+Inf\",} 1\n" +
                "histogram_sum{} 0.75\nhistogram_count{} 1\n" +
                "\n" +
                "# TYPE histogram1 histogram\n" +
                "histogram1_bucket{le=\"1\",} 1\n" +
                "histogram1_bucket{le=\"2\",} 1\n" +
                "histogram1_bucket{le=\"4\",} 1\n" +
                "histogram1_bucket{le=\"+Inf\",} 1\n" +
                "histogram1_sum{} 0.75\nhistogram1_count{} 1\n" +
                "\n" +
                "# TYPE histogramExponential histogram\n" +
                "histogramExponential_bucket{le=\"1\",} 0\n" +
                "histogramExponential_bucket{le=\"2\",} 0\n" +
                "histogramExponential_bucket{le=\"4\",} 1\n" +
                "histogramExponential_bucket{le=\"8\",} 1\n" +
                "histogramExponential_bucket{le=\"+Inf\",} 1\n" +
                "histogramExponential_sum{} 3\n" +
                "histogramExponential_count{} 1\n" +
                "\n", scrape);
        }
    }
}
