namespace Vecc.Prometheus.Internal
{
    /// <summary>
    ///     Creates the exponential values for the histogram buckets.
    /// </summary>
    public interface IHistogramBucketFactory
    {
        /// <summary>
        ///     Gets a specified number of exponential numbers.
        /// </summary>
        /// <param name="start">Number to start the exponential count from.</param>
        /// <param name="exponential">Exponent to get the buckets for.</param>
        /// <param name="count">Number of items to return.</param>
        /// <returns>A list of exponential numbers.</returns>
        double[] GetExponential(double start, double exponential, int count);
    }
}
