using System;
using System.Collections.Generic;
using System.Text;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class MetricFactoryTests
    {
        [Fact]
        public void GetCounterReturnsCounter()
        {
            var factory = new MetricFactory();
            var metric = factory.GetCounter("test", new[] { "label1" }, "help");
            Assert.Equal("test", metric.Name);
            Assert.Equal(new[] { "label1" }, metric.Labels);
            Assert.Equal("help", metric.Help);
            Assert.Equal(MetricType.Counter, metric.Type);
        }

        [Fact]
        public void GetGaugeReturnsGauge()
        {
            var factory = new MetricFactory();
            var metric = factory.GetGauge("test", new[] { "label1" }, "help");
            Assert.Equal("test", metric.Name);
            Assert.Equal(new[] { "label1" }, metric.Labels);
            Assert.Equal("help", metric.Help);
            Assert.Equal(MetricType.Gauge, metric.Type);
        }

        [Fact]
        public void GetHistogramReturnsGauge()
        {
            var factory = new MetricFactory();
            var metric = factory.GetHistogram("test", new[] { "label1" }, "help");
            Assert.Equal("test", metric.Name);
            Assert.Equal(new[] { "label1" }, metric.Labels);
            Assert.Equal("help", metric.Help);
            Assert.Equal(MetricType.Histogram, metric.Type);
        }
    }
}
