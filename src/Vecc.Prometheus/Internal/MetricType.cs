namespace Vecc.Prometheus.Internal
{
    /// <summary>
    ///     Prometheus metric types
    /// </summary>
    public enum MetricType
    {
        /// <summary>
        ///     Counter
        /// </summary>
        Counter,

        /// <summary>
        ///     Gauge
        /// </summary>
        Gauge,

        /// <summary>
        ///     Histogram
        /// </summary>
        Histogram,

        /// <summary>
        ///     Untyped
        /// </summary>
        Untyped
    }
}
