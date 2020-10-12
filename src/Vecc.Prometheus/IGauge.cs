namespace Vecc.Prometheus
{
    /// <summary>
    ///     Gauge based metric for Prometheus. Represents an absolute or current value.
    /// </summary>
    public interface IGauge : IMetric
    {
        /// <summary>
        ///     Changes the gauge value by the specified amount.
        /// </summary>
        /// <param name="amount">Amount to change the value by.</param>
        /// <returns>The new value of the gauge.</returns>
        double Add(double amount);

        /// <summary>
        ///     Sets the gauge to the specified value.
        /// </summary>
        /// <param name="value"></param>
        void Set(double value);

        /// <summary>
        ///     Gets the current value of the gauge.
        /// </summary>
        double Value { get; }
    }
}
