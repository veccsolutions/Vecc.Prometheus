using System;
using System.IO;
using System.Threading.Tasks;
using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class CounterTests
    {
        [Fact]
        public async Task ScrapeReturnsCorrectWithIdentifier()
        {
            var counter = new Counter("test", "test");
            counter.Increment(25);
            var stringWriter = new StringWriter();
            await counter.ScrapeAsync(stringWriter);
            Assert.Equal("test{test} 25\n", stringWriter.ToString());
        }

        [Fact]
        public async Task ScrapeReturnsCorrectWithoutIdentifier()
        {
            var counter = new Counter(null, "test");
            counter.Increment(25);
            var stringWriter = new StringWriter();
            await counter.ScrapeAsync(stringWriter);
            Assert.Equal("test 25\n", stringWriter.ToString());

            counter = new Counter(string.Empty, "test");
            counter.Increment(25);
            stringWriter = new StringWriter();
            await counter.ScrapeAsync(stringWriter);
            Assert.Equal("test 25\n", stringWriter.ToString());
        }

        [Fact]
        public void IncrementAddsToValue()
        {
            var counter = new Counter("test", "test");
            Assert.Equal(0, counter.Value);
            counter.Increment(25);
            Assert.Equal(25, counter.Value);
        }

        [Fact]
        public void IncrementOnlyAllowsPositiveNumbersOrZero()
        {
            var counter = new Counter("test", "test");
            Assert.Throws<ArgumentOutOfRangeException>(() => counter.Increment(-1));
            counter.Increment(0);
            counter.Increment(1);
        }

        [Fact]
        public async Task CallbacksAreExecuted()
        {
            var called = false;
            var counter = new Counter("test", "test");
            counter.AddBeforeScrapeCallback((metric) =>
            {
                Assert.Same(counter, metric);
                called = true;
                return Task.CompletedTask;
            });
            await counter.ScrapeAsync(new StringWriter());
            Assert.True(called);
        }
    }
}
