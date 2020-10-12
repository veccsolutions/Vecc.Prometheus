using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vecc.Prometheus.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceProviderFactory.GetServiceProvider();
            var registry = serviceProvider.GetRequiredService<IMetricRegistry>();
            registry.RegisterCoreStatistics();

            var counter = registry.GetCounter("QueryCounts", "Times a query was ran", new[] { "QueryName" });
            var gauge = registry.GetGauge("LoggedIn", "Current user counts", new[] { "UserType" });
            var histogram = registry.GetHistogram("QueryTime", "Time it takes to run a query", new[] { "QeuryName" }, 1, 2, 5, 10, 100);

            counter.Get("SelectUser").Increment(1);
            counter.Get("SelectUser").Increment(1);
            counter.Get("SelectUser").Increment(1);
            counter.Get("InsertUser").Increment(1);
            gauge.Get("Users").Set(10);
            gauge.Get("Bots").Set(1);
            histogram.Get("SelectUser").Add(1);
            histogram.Get("SelectUser").Add(1);
            histogram.Get("SelectUser").Add(1);
            histogram.Get("SelectUser").Add(1);
            histogram.Get("SelectUser").Add(2);
            histogram.Get("InsertUser").Add(20);

            var scrapeResult = registry.ScrapeAsync().Result;
            Console.WriteLine(scrapeResult);
        }
    }
}
