using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Vecc.Prometheus
{
    /// <summary>
    ///     Extension methods for <seealso cref="IMetricRegistry"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class RegistryExtensions
    {
        /// <summary>
        ///     Register basic process level information in a metric registry.
        /// </summary>
        /// <param name="registry">Registry to add the metrics to.</param>
        public static void RegisterCoreStatistics(this IMetricRegistry registry)
        {
            var collectionCounter = registry.GetCounter("dotnet_collection_count_total", "GC collection count", new[] { "generation" });
            var collectionCounters = Enumerable.Range(0, GC.MaxGeneration + 1).Select(generation => collectionCounter.Get(generation.ToString())).ToArray();
            var startTimeGauge = registry.GetGauge("process_start_time_seconds", "Start time of the process since unix epoch in seconds.").Get();
            var cpuTotalCounter = registry.GetCounter("process_cpu_seconds_total", "Total user and system CPU time spent in seconds.").Get();
            var virtualMemorySizeGauge = registry.GetGauge("process_virtual_memory_bytes", "Virtual memory size in bytes.").Get();
            var workingSetGauge = registry.GetGauge("process_working_set_bytes", "Process working set").Get();
            var privateMemorySizeGauge = registry.GetGauge("process_private_memory_bytes", "Process private memory size").Get();
            var openHandlesGauge = registry.GetGauge("process_open_handles", "Number of open handles").Get();
            var numThreadsGauge = registry.GetGauge("process_num_threads", "Total number of threads").Get();

            // .net specific metrics
            var totalMemoryGauge = registry.GetGauge("dotnet_total_memory_bytes", "Total known allocated memory").Get();
            var process = Process.GetCurrentProcess();

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            startTimeGauge.Set((process.StartTime.ToUniversalTime() - epoch).TotalSeconds);

            registry.AddBeforeScrapeCallback((r) =>
            {
                process.Refresh();

                for (var generation = 0; generation <= GC.MaxGeneration; generation++)
                {
                    var counter = collectionCounters[generation];
                    counter.Increment(GC.CollectionCount(generation) - counter.Value);
                }

                totalMemoryGauge.Set(GC.GetTotalMemory(false));
                virtualMemorySizeGauge.Set(process.VirtualMemorySize64);
                workingSetGauge.Set(process.WorkingSet64);
                privateMemorySizeGauge.Set(process.PrivateMemorySize64);
                cpuTotalCounter.Increment(Math.Max(0, process.TotalProcessorTime.TotalSeconds - cpuTotalCounter.Value));
                openHandlesGauge.Set(process.HandleCount);
                numThreadsGauge.Set(process.Threads.Count);

                return Task.CompletedTask;
            });
        }
    }
}
