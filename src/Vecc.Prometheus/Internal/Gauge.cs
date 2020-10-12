using System.IO;
using System.Threading.Tasks;

namespace Vecc.Prometheus.Internal
{
    public class Gauge : MetricBase, IGauge
    {
        private readonly string _identifier;
        private readonly object _locker;
        private readonly string _name;

        public Gauge(string identifier, string name)
        {
            this._locker = new object();
            this._name = name;

            if (string.IsNullOrWhiteSpace(identifier))
            {
                this._identifier = string.Empty;
            }
            else
            {
                this._identifier = "{" + identifier + "}";
            }
        }

        public double Value { get; private set; }

        public double Add(double amount)
        {
            lock (this._locker)
            {
                this.Value += amount;

                this.Expose = true;

                return this.Value;
            }
        }

        public void Set(double value)
        {
            lock (this._locker)
            {
                this.Value = value;

                this.Expose = true;
            }
        }

        public async Task ScrapeAsync(TextWriter writer)
        {
            foreach (var callback in this.BeforeScrapeCallbacks)
            {
                await callback(this);
            }

            if (!this.Expose)
            {
                return;
            }

            lock (this._locker)
            {
                this.Expose = false;

                writer.Write($"{this._name}{this._identifier} {this.Value}\n");
            }
        }
    }
}
