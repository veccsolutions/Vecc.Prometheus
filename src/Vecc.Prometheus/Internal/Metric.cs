using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vecc.Prometheus.Internal
{
    public class Metric<TMetricType> : IMetric<TMetricType>
        where TMetricType : IMetric
    {
        private readonly Factory _metricBuilder;
        private readonly ConcurrentDictionary<string, TMetricType> _metrics;
        private readonly List<Func<IMetric<TMetricType>, Task>> _beforeScrapeCallbacks;

        public delegate TMetricType Factory(string identifier);

        public Metric(string help, string[] labels, string name, MetricType type, Factory factory)
        {
            this._beforeScrapeCallbacks = new List<Func<IMetric<TMetricType>, Task>>();
            this._metricBuilder = factory;
            this._metrics = new ConcurrentDictionary<string, TMetricType>();

            this.Help = help;
            this.Labels = labels;
            this.Name = this.SanitizeName(name);
            this.Type = type;
        }

        public virtual string Help { get; protected set; }

        public string[] Labels { get; protected set; }

        public string Name { get; protected set; }

        public MetricType Type { get; }

        public void AddBeforeScrapeCallback(Func<IMetric<TMetricType>, Task> callback)
            => this._beforeScrapeCallbacks.Add(callback);

        public virtual TMetricType Get(params string[] labelValues)
        {
            var identifier = this.GetIdentifier(labelValues);
            var result = this._metrics.GetOrAdd(identifier, (i) => this._metricBuilder(identifier));

            return result;
        }

        public string GetIdentifier(string[] labelValues)
        {
            if (labelValues == null)
            {
                labelValues = new string[0];
            }

            if (labelValues.Length != this.Labels.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(labelValues), "Label value length must match the length of the labels");
            }

            if (labelValues.Length == 0)
            {
                return string.Empty;
            }

            var labels = this.Labels;
            var values = new List<string>();

            for (var x = 0; x < labelValues.Length; x++)
            {
                var escaped = labelValues[x]
                        .Replace(@"\", @"\\") // \  -> \\
                        .Replace("\"", "\\\"")// "  -> \"
                        .Replace("\n", @"\n");// \n -> \\n
                values.Add($"{labels[x]}=\"{escaped}\"");
            }

            var result = string.Join(",", values);
            return result;
        }

        public async Task ScrapeAsync(TextWriter textWriter)
        {
            foreach (var callback in this._beforeScrapeCallbacks)
            {
                await callback(this);
            }

            var keys = this._metrics.Keys;
            if (!string.IsNullOrWhiteSpace(this.Help))
            {
                textWriter.Write("# HELP {0}\n", this.Help);
            }

            if (this.Type != MetricType.Untyped)
            {
                string type;
                switch (this.Type)
                {
                    case MetricType.Counter:
                        type = "counter";
                        break;
                    case MetricType.Gauge:
                        type = "gauge";
                        break;
                    case MetricType.Histogram:
                        type = "histogram";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(this.Type), "Type is not an expected metric type");
                }

                textWriter.Write("# TYPE {0} {1}\n", this.Name, type);
            }

            foreach (var key in keys)
            {
                var metric = this._metrics[key];

                await metric.ScrapeAsync(textWriter);
            }

            textWriter.Write('\n');
        }

        private string SanitizeName(string name)
        {
            // Only allow alphanumeric, colon and _
            // everything else gets turned into an _

            var result = string.Concat(name.Select(x =>
            {
                if ((x >= '0' && x <= '9') ||
                    (x == ':') ||
                    (x == '_') ||
                    (x >= 'A' && x <= 'Z') ||
                    (x >= 'a' && x <= 'z'))
                {
                    return x;
                }

                return '_';
            }));

            return result;
        }
    }
}
