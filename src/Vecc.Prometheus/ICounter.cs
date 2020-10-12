namespace Vecc.Prometheus
{
    /// <summary>
    ///     Counter based metric for Prometheus. Represents an increasing counter. It must only increase and can not decrease or go below 0.
    /// </summary>
    public interface ICounter : IMetric
    {
        /// <summary>
        ///     Add the specified amount to current value.
        /// </summary>
        /// <param name="amount">Amount to add, must be greater than or equal to 0.</param>
        /// <returns>The new value assigned to the counter</returns>
        double Increment(double amount = 1.0);

        /// <summary>
        ///     Gets the current value of the counter.
        /// </summary>
        double Value { get; }
    }
}
