using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Vecc.Prometheus.Internal
{
    public abstract class MetricBase
    {
        private volatile bool _expose = false;

        public MetricBase()
        {
            this.BeforeScrapeCallbacks = new ConcurrentBag<Func<IMetric, Task>>();
        }

        public bool Expose
        {
            get => this._expose;
            protected set => this._expose = value;
        }

        protected ConcurrentBag<Func<IMetric, Task>> BeforeScrapeCallbacks { get; }

        public void AddBeforeScrapeCallback(Func<IMetric, Task> callback)
            => this.BeforeScrapeCallbacks.Add(callback);

        public double AddDouble(double current, double amount)
        {
            var result = current + amount;

            // Have we hit the max decent precision of double, if so, rollover.
            if (result == double.MaxValue)
            {
                //rollover
                result = amount;
            }

            return result;
        }
    }
}
