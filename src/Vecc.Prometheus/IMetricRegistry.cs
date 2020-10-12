using System;
using System.Threading.Tasks;

namespace Vecc.Prometheus
{
    /// <summary>
    ///     Cantainer for all of the metrics in a scrape.
    /// </summary>
    public interface IMetricRegistry
    {
        /// <summary>
        ///     Add method to call before the scrape starts.
        /// </summary>
        /// <param name="callback">Method to call.</param>
        void AddBeforeScrapeCallback(Func<IMetricRegistry, Task> callback);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <returns></returns>
        IMetric<ICounter> GetCounter(string name, string help = null, string[] labels = null);

        /// <summary>
        ///     Gets a <seealso cref="IGauge"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <returns></returns>
        IMetric<IGauge> GetGauge(string name, string help = null, string[] labels = null);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="buckets">Available buckets in the histogram.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogram(string name, params double[] buckets);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="buckets">Available buckets in the histogram.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogram(string name, string[] labels, params double[] buckets);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <param name="buckets">Available buckets in the histogram.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogram(string name, string help, params double[] buckets);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <param name="buckets">Available buckets in the histogram.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogram(string name, string help, string[] labels, params double[] buckets);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="start">First bucket value.</param>
        /// <param name="exponential">Exponential value.</param>
        /// <param name="count">Number of exponential buckets to include.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogramExponential(string name, double start, double exponential, int count);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="start">First bucket value.</param>
        /// <param name="exponential">Exponential value.</param>
        /// <param name="count">Number of exponential buckets to include.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogramExponential(string name, string[] labels, double start, double exponential, int count);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <param name="start">First bucket value.</param>
        /// <param name="exponential">Exponential value.</param>
        /// <param name="count">Number of exponential buckets to include.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogramExponential(string name, string help, double start, double exponential, int count);

        /// <summary>
        ///     Gets a <seealso cref="ICounter"/> metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="help">Helper text for Prometheus.</param>
        /// <param name="labels">Available labels for the metric.</param>
        /// <param name="start">First bucket value.</param>
        /// <param name="exponential">Exponential value.</param>
        /// <param name="count">Number of exponential buckets to include.</param>
        /// <returns></returns>
        IMetric<IHistogram> GetHistogramExponential(string name, string help, string[] labels, double start, double exponential, int count);

        /// <summary>
        ///     Adds a counter metric to the scrape.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="counter">The metric to add.</param>
        void RegisterCounter(string name, IMetric<ICounter> counter);

        /// <summary>
        ///     Adds a gauge metric to the scrape.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="gauge">The metric to add.</param>
        void RegisterGauge(string name, IMetric<IGauge> gauge);

        /// <summary>
        ///     Adds a histogram metric to the scrape.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="histogram">The metric to add.</param>
        void RegisterHistogram(string name, IMetric<IHistogram> histogram);

        /// <summary>
        ///     Asynchronously create the Prometheus scrape.
        /// </summary>
        /// <returns>The text that Prometheus reads.</returns>
        Task<string> ScrapeAsync();
    }
}
