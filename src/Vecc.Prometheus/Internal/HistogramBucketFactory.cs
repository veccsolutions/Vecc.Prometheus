using System;
using System.Collections.Generic;
using System.Linq;

namespace Vecc.Prometheus.Internal
{
    public class HistogramBucketFactory : IHistogramBucketFactory
    {
        private readonly Dictionary<double, Dictionary<double, Dictionary<int, double[]>>> _exponentialCache;

        public HistogramBucketFactory()
        {
            this._exponentialCache = new Dictionary<double, Dictionary<double, Dictionary<int, double[]>>>();
        }

        public double[] GetExponential(double start, double exponential, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Must be above zero.");
            }

            if (start <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Must be above zero.");
            }

            if (exponential <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(exponential), "Must be above 1");
            }

            if (!this._exponentialCache.TryGetValue(start, out var startCache))
            {
                startCache = new Dictionary<double, Dictionary<int, double[]>>();
                this._exponentialCache[start] = startCache;
            }

            if (!startCache.TryGetValue(exponential, out var exponentialCache))
            {
                exponentialCache = new Dictionary<int, double[]>();
                startCache[exponential] = exponentialCache;
            }

            if (!exponentialCache.TryGetValue(count, out var result))
            {
                result = this.BuildExponentials(start, exponential, count);
                exponentialCache[count] = result;
            }

            return result;
        }

        private double[] BuildExponentials(double start, double exponential, int count)
        {
            var current = start;
            var result = new List<double>() { current };

            result.AddRange(Enumerable.Range(1, count - 1).Select((_) => current *= exponential));

            return result.ToArray();
        }
    }
}
