using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vecc.Prometheus.Internal
{
    public class MetricRegistry : IMetricRegistry
    {
        private readonly ConcurrentBag<Func<IMetricRegistry, Task>> _beforeScrapeCallbacks;
        private readonly ConcurrentDictionary<string, IMetric<ICounter>> _counters;
        private readonly ConcurrentDictionary<string, IMetric<IGauge>> _gauges;
        private readonly ConcurrentDictionary<string, IMetric<IHistogram>> _histograms;
        private readonly object _locker;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHistogramBucketFactory _histogramBucketFactory;

        public MetricRegistry(IDateTimeProvider dateTimeProvider, IHistogramBucketFactory histogramBucketFactory, IMetricFactory metricFactory)
        {
            this._beforeScrapeCallbacks = new ConcurrentBag<Func<IMetricRegistry, Task>>();
            this._counters = new ConcurrentDictionary<string, IMetric<ICounter>>();
            this._gauges = new ConcurrentDictionary<string, IMetric<IGauge>>();
            this._histograms = new ConcurrentDictionary<string, IMetric<IHistogram>>();
            this._locker = new object();

            this.Factory = metricFactory;
            this._dateTimeProvider = dateTimeProvider;
            this._histogramBucketFactory = histogramBucketFactory;
        }

        public IMetricFactory Factory { get; }

        public void AddBeforeScrapeCallback(Func<IMetricRegistry, Task> callback)
            => this._beforeScrapeCallbacks.Add(callback);

        public IMetric<ICounter> GetCounter(string name, string help = null, string[] labels = null)
        {
            lock (this._locker)
            {
                if (!this._counters.TryGetValue(name, out var result))
                {
                    result = this.Factory.GetCounter(name, labels, help);
                    this._counters.TryAdd(name, result);
                }

                return result;
            }
        }

        public IMetric<IGauge> GetGauge(string name, string help = null, string[] labels = null)
        {
            lock (this._locker)
            {
                if (!this._gauges.TryGetValue(name, out var result))
                {
                    result = this.Factory.GetGauge(name, labels, help);
                    this._gauges.TryAdd(name, result);
                }

                return result;
            }
        }

        public IMetric<IHistogram> GetHistogram(string name, params double[] buckets)
            => this.GetHistogram(name, null, null, buckets);

        public IMetric<IHistogram> GetHistogram(string name, string help, params double[] buckets)
            => this.GetHistogram(name, help, null, buckets);

        public IMetric<IHistogram> GetHistogram(string name, string[] labels, params double[] buckets)
            => this.GetHistogram(name, null, labels, buckets);

        public IMetric<IHistogram> GetHistogram(string name, string help, string[] labels, params double[] buckets)
        {
            lock (this._locker)
            {
                if (!this._histograms.TryGetValue(name, out var result))
                {
                    result = this.Factory.GetHistogram(name, labels, help, buckets);
                    this._histograms.TryAdd(name, result);
                }

                return result;
            }
        }

        public void RegisterCounter(string name, IMetric<ICounter> counter)
        {
            lock (this._locker)
            {
                this._counters.TryAdd(name, counter);
            }
        }

        public void RegisterGauge(string name, IMetric<IGauge> gauge)
        {
            lock (this._locker)
            {
                this._gauges.TryAdd(name, gauge);
            }
        }

        public void RegisterHistogram(string name, IMetric<IHistogram> histogram)
        {
            lock (this._locker)
            {
                this._histograms.TryAdd(name, histogram);
            }
        }

        public async Task<string> ScrapeAsync()
        {
            string[] counterKeys;
            string[] gaugeKeys;
            string[] histogramKeys;

            foreach (var callback in this._beforeScrapeCallbacks)
            {
                await callback(this);
            }

            lock (this._locker)
            {
                counterKeys = this._counters.Keys.OrderBy(x => x).ToArray();
                gaugeKeys = this._gauges.Keys.OrderBy(x => x).ToArray();
                histogramKeys = this._histograms.Keys.OrderBy(x => x).ToArray();
            }

            var scrape = new StringBuilder();
            scrape.AppendFormat("# Scraped at {0}\n", this._dateTimeProvider.OffsetNow);

            using (var textWriter = new StringWriter(scrape))
            {
                foreach (var counter in counterKeys)
                {
                    await this._counters[counter].ScrapeAsync(textWriter);
                }

                foreach (var gauge in gaugeKeys)
                {
                    await this._gauges[gauge].ScrapeAsync(textWriter);
                }

                foreach (var histogram in histogramKeys)
                {
                    await this._histograms[histogram].ScrapeAsync(textWriter);
                }
            }

            var result = scrape.ToString();
            return result;
        }

        public IMetric<IHistogram> GetHistogramExponential(string name, double start, double exponential, int count) =>
            this.GetHistogramExponential(name, null, null, start, exponential, count);

        public IMetric<IHistogram> GetHistogramExponential(string name, string[] labels, double start, double exponential, int count) =>
            this.GetHistogramExponential(name, null, labels, start, exponential, count);

        public IMetric<IHistogram> GetHistogramExponential(string name, string help, double start, double exponential, int count) =>
            this.GetHistogramExponential(name, help, null, start, exponential, count);

        public IMetric<IHistogram> GetHistogramExponential(string name, string help, string[] labels, double start, double exponential, int count) =>
            this.GetHistogram(name, help, labels, this._histogramBucketFactory.GetExponential(start, exponential, count));
    }
}
