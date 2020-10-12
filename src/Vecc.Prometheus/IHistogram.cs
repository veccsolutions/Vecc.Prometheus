namespace Vecc.Prometheus
{
    /// <summary>
    ///     Histogram metric in Prometheus. Represents additive histogram metrics.
    /// </summary>
    public interface IHistogram : IMetric
    {
        /// <summary>
        ///     Adds a value to the appropriate histogram bucket.
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <param name="count">Count of times this value should be added.</param>
        void Add(double value, double count = 1);
    }
}
