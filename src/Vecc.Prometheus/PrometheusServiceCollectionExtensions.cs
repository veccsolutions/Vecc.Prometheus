using System.Diagnostics.CodeAnalysis;
using Vecc.Prometheus;
using Vecc.Prometheus.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class PrometheusServiceCollectionExtensions
    {
        /// <summary>
        ///     Add a single metric registry and required classes for exposing a Prometheus scrape endpoint.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddPrometheus(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            serviceCollection.AddSingleton<IHistogramBucketFactory, HistogramBucketFactory>();
            serviceCollection.AddSingleton<IMetricFactory, MetricFactory>();
            serviceCollection.AddSingleton<IMetricRegistry, MetricRegistry>();

            return serviceCollection;
        }
    }
}
