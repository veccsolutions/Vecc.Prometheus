using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vecc.Prometheus.Internal
{
    public class Histogram : MetricBase, IHistogram
    {
        private readonly ConcurrentDictionary<double, double> _buckets;
        private double _count;
        private readonly string _identifier;
        private readonly object _locker;
        private readonly string _name;
        private double _total;

        public Histogram(string identifier, string name, double[] buckets)
        {
            this._locker = new object();
            this._identifier = identifier;
            this._name = name;

            var bucketDictionary = buckets.Distinct().ToDictionary(x => x, x => 0.0);
            this._buckets = new ConcurrentDictionary<double, double>(bucketDictionary);
        }

        public void Add(double amount, double count = 1)
        {
            lock (this._locker)
            {
                foreach (var kvp in this._buckets)
                {
                    if (kvp.Key > amount)
                    {
                        var newValue = this.AddDouble(kvp.Value, count);
                        this._buckets[kvp.Key] += count;
                    }
                }

                var currentCount = this.AddDouble(this._count, 1);
                var currentTotal = this.AddDouble(this._total, amount);

                this._count = currentCount;
                this._total = currentTotal;
                this.Expose = true;
            }
        }

        public Dictionary<double, double> GetBuckets() => this._buckets.ToDictionary(x => x.Key, x => x.Value);

        public async Task ScrapeAsync(TextWriter writer)
        {
            foreach (var callback in this.BeforeScrapeCallbacks)
            {
                await callback(this);
            }

            if (!this.Expose)
            {
                return;
            }

            lock (this._locker)
            {
                this.Expose = false;

                foreach (var bucket in this._buckets.OrderBy(x => x.Key))
                {
                    writer.Write($"{this._name}_bucket{{le=\"{bucket.Key}\",{this._identifier}}} {bucket.Value}\n");
                }

                writer.Write($"{this._name}_bucket{{le=\"+Inf\",{this._identifier}}} {this._count}\n");
                writer.Write($"{this._name}_sum{{{this._identifier}}} {this._total}\n");
                writer.Write($"{this._name}_count{{{this._identifier}}} {this._count}\n");
            }
        }
    }
}
