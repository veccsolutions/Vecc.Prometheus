namespace Vecc.Prometheus.Internal
{
    public class MetricFactory : IMetricFactory
    {
        public IMetric<ICounter> GetCounter(string name, string[] labels, string help)
        {
            var result = new Metric<ICounter>(help, labels ?? new string[0], name, MetricType.Counter,
                    new Metric<ICounter>.Factory((identifier) => new Counter(identifier, name)));

            return result;
        }

        public IMetric<IGauge> GetGauge(string name, string[] labels, string help)
        {
            var result = new Metric<IGauge>(help, labels ?? new string[0], name, MetricType.Gauge,
                    new Metric<IGauge>.Factory((identifier) => new Gauge(identifier, name)));

            return result;
        }

        public IMetric<IHistogram> GetHistogram(string name, string[] labels, string help, params double[] buckets)
        {
            var result = new Metric<IHistogram>(help, labels ?? new string[0], name, MetricType.Histogram,
                    new Metric<IHistogram>.Factory((identifier) => new Histogram(identifier, name, buckets)));

            return result;
        }
    }
}
