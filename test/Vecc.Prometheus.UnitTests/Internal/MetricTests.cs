using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class MetricTests
    {
        [Fact]
        public async Task MetricCallbacksAreCalled()
        {
            var called = false;
            var metric = new Metric<Counter>(null, new string[0], "test", MetricType.Counter, (i) => new Counter(i, "test"));
            metric.AddBeforeScrapeCallback((metric) =>
            {
                called = true;
                return Task.CompletedTask;
            });
            metric.Get().Increment();
            var textWriter = new StringWriter();
            await metric.ScrapeAsync(textWriter);
            Assert.True(called);
            Assert.Equal("# TYPE test counter\n" +
                "test 1\n" +
                "\n", textWriter.ToString());
        }

        [Fact]
        public async Task CounterMetricIsInResult()
        {
            var metric = new Metric<Counter>(null, new string[0], "test", MetricType.Counter, (i) => new Counter(i, "test"));
            var textWriter = new StringWriter();
            metric.Get().Increment(25);
            await metric.ScrapeAsync(textWriter);
            Assert.Equal("# TYPE test counter\n" +
                "test 25\n" +
                "\n", textWriter.ToString());
        }

        [Fact]
        public async Task GaugeMetricIsInResult()
        {
            var metric = new Metric<Gauge>(null, new string[0], "test", MetricType.Gauge, (i) => new Gauge(i, "test"));
            var textWriter = new StringWriter();
            metric.Get().Add(25);
            await metric.ScrapeAsync(textWriter);
            Assert.Equal("# TYPE test gauge\n" +
                "test 25\n" +
                "\n", textWriter.ToString());
        }

        [Fact]
        public async Task HistogramMetricIsInResult()
        {
            var metric = new Metric<Histogram>(null, new string[0], "test", MetricType.Histogram, (i) => new Histogram(i, "test", new[] { 1D, 2D, 4D }));
            metric.Get().Add(3);
            var textWriter = new StringWriter();
            await metric.ScrapeAsync(textWriter);
            Assert.Equal("# TYPE test histogram\n" +
                "test_bucket{le=\"1\",} 0\n" +
                "test_bucket{le=\"2\",} 0\n" +
                "test_bucket{le=\"4\",} 1\n" +
                "test_bucket{le=\"+Inf\",} 1\n" +
                "test_sum{} 3\n" +
                "test_count{} 1\n" +
                "\n", textWriter.ToString());
        }

        [Fact]
        public void SanitizeNameRemovesBogusCharacters()
        {
            var metric = new Metric<Counter>(null, new string[0], "test#!@$%^", MetricType.Counter, (i) => new Counter(i, "test"));
            Assert.Equal("test______", metric.Name);
        }

        [Fact]
        public void GetIdentifierNullReturnsEmpty()
        {
            var metric = new Metric<Counter>(null, new string[0], "test", MetricType.Counter, (i) => new Counter(i, "test"));
            var actual = metric.GetIdentifier(null);
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void GetIdentifierEmptyArrayReturnsEmpty()
        {
            var metric = new Metric<Counter>(null, new string[0], "test", MetricType.Counter, (i) => new Counter(i, "test"));
            var actual = metric.GetIdentifier(new string[0]);
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void GetIdentifierInconsistentLabelLengthThrowOutOfRange()
        {
            var metric = new Metric<Counter>(null, new string[0], "test", MetricType.Counter, (i) => new Counter(i, "test"));
            Assert.Throws<ArgumentOutOfRangeException>("labelValues", () => metric.GetIdentifier(new[] { "test" }));
        }

        [Fact]
        public void GetIdentifierReturnsCorrect()
        {
            var metric = new Metric<Counter>(null, new[] { "text1", "text2" }, "test", MetricType.Counter, (i) => new Counter(i, "test"));
            var actual = metric.GetIdentifier(new[] { "hello", "test" });
            Assert.Equal("text1=\"hello\",text2=\"test\"", actual);
        }
    }
}
