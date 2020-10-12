namespace Vecc.Prometheus.Internal
{
    /// <summary>
    ///     Creates the individual type of metrics.
    /// </summary>
    public interface IMetricFactory
    {
        /// <summary>
        ///     Creates a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <returns></returns>
        IMetric<ICounter> GetCounter(string name, string[] labels, string help);

        /// <summary>
        ///     Creates a <seealso cref="IGauge"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <returns></returns>
        IMetric<IGauge> GetGauge(string name, string[] labels, string help);

        /// <summary>
        ///     Creates a <seealso cref="IHistogram"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <param name="buckets">Available buckets in the histogram.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogram(string name, string[] labels, string help, params double[] buckets);
    }
}
