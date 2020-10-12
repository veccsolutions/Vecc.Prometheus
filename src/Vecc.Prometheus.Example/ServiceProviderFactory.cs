using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vecc.Prometheus.Example
{
    public class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddPrometheus();

            return services.BuildServiceProvider();
        }
    }
}
