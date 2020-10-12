using System;
using System.IO;
using System.Threading.Tasks;
using Vecc.Prometheus.Internal;

namespace Vecc.Prometheus
{
    /// <summary>
    ///     Base Prometheus metric.
    /// </summary>
    public interface IMetric
    {
        /// <summary>
        ///     Gets whether the metric should be exposed in the next scrape.
        /// </summary>
        bool Expose { get; }

        /// <summary>
        ///     A method to call before the scrape is executed.
        /// </summary>
        /// <param name="callback"></param>
        void AddBeforeScrapeCallback(Func<IMetric, Task> callback);

        /// <summary>
        ///     Asynchronously write out the metric to the scrape.
        /// </summary>
        /// <param name="textWriter">The <seealso cref="TextWriter"/> to write the value to.</param>
        /// <returns></returns>
        Task ScrapeAsync(TextWriter textWriter);
    }

    /// <summary>
    ///     The container for a specific metric.
    /// </summary>
    /// <typeparam name="TMetricType">The type of <seealso cref="IMetric"/> that this container exposes.</typeparam>
    public interface IMetric<TMetricType>
        where TMetricType : IMetric
    {
        /// <summary>
        ///     Helper text to display in Prometheus.
        /// </summary>
        string Help { get; }

        /// <summary>
        ///     Gets the available labels to use in the metric.
        /// </summary>
        string[] Labels { get; }

        /// <summary>
        ///     Gets the name of the metric.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the type of the metric exposed.
        /// </summary>
        MetricType Type { get; }

        /// <summary>
        ///     Add method to call before the scrape starts.
        /// </summary>
        /// <param name="callback">Method to call.</param>
        void AddBeforeScrapeCallback(Func<IMetric<TMetricType>, Task> callback);

        /// <summary>
        ///     Get a metric value with the specified <seealso cref="Labels"/>
        /// </summary>
        /// <param name="labelValues"></param>
        /// <returns></returns>
        TMetricType Get(params string[] labelValues);

        /// <summary>
        ///     Asynchronously writes the metrics to the scrape.
        /// </summary>
        /// <param name="textWriter">The <seealso cref="TextWriter"/> to write the value to.</param>
        /// <returns></returns>
        Task ScrapeAsync(TextWriter textWriter);
    }
}
