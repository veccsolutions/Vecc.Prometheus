using System.IO;
using System.Threading.Tasks;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class GaugeTests
    {
        [Fact]
        public async Task ScrapeReturnsCorrectWithIdentifier()
        {
            var gauge = new Gauge("test", "test");
            gauge.Add(25);
            var stringWriter = new StringWriter();
            await gauge.ScrapeAsync(stringWriter);
            Assert.Equal("test{test} 25\n", stringWriter.ToString());
        }

        [Fact]
        public async Task ScrapeReturnsCorrectWithoutIdentifier()
        {
            var gauge = new Gauge(null, "test");
            gauge.Add(25);
            var stringWriter = new StringWriter();
            await gauge.ScrapeAsync(stringWriter);
            Assert.Equal("test 25\n", stringWriter.ToString());

            gauge = new Gauge(string.Empty, "test");
            gauge.Add(25);
            stringWriter = new StringWriter();
            await gauge.ScrapeAsync(stringWriter);
            Assert.Equal("test 25\n", stringWriter.ToString());
        }

        [Fact]
        public void IncrementAddsToValue()
        {
            var counter = new Gauge("test", "test");
            Assert.Equal(0, counter.Value);
            counter.Add(25);
            Assert.Equal(25, counter.Value);
        }

        [Fact]
        public void SetSetsValue()
        {
            var counter = new Gauge("test", "test");
            Assert.Equal(0, counter.Value);
            counter.Add(25);
            counter.Set(10);
            Assert.Equal(10, counter.Value);
        }

        [Fact]
        public async Task CallbacksAreExecuted()
        {
            var called = false;
            var gauge = new Gauge("test", "test");
            gauge.AddBeforeScrapeCallback((metric) =>
            {
                Assert.Same(gauge, metric);
                called = true;
                return Task.CompletedTask;
            });
            await gauge.ScrapeAsync(new StringWriter());
            Assert.True(called);
        }
    }
}
