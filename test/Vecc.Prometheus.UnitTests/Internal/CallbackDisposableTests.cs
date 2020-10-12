using Vecc.Prometheus.Internal;
using Xunit;

namespace Vecc.Prometheus.UnitTests.Internal
{
    public class CallbackDisposableTests
    {
        [Fact]
        public void DisposeCallsCallback()
        {
            var called = false;
            var disposable = new CallbackDisposable(() => called = true);
            disposable.Dispose();
            Assert.True(called);
        }

        [Fact]
        public void CallbackCalledOnlyOnce()
        {
            var calledCount = 0;
            var disposable = new CallbackDisposable(() => calledCount++);
            disposable.Dispose();
            disposable.Dispose();
            Assert.Equal(1, calledCount);
        }
    }
}
