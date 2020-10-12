using System;
using System.IO;
using System.Threading.Tasks;

namespace Vecc.Prometheus.Internal
{
    public class Counter : MetricBase, ICounter
    {
        private readonly string _identifier;
        private readonly object _locker;
        private readonly string _name;

        public Counter(string identifier, string name)
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

        public double Increment(double amount = 1.0)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than or equal to 0");
            }

            double result;

            lock (this._locker)
            {
                result = this.AddDouble(this.Value, amount);

                this.Value = result;

                this.Expose = true;
            }

            return result;
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

                writer.Write("{0}{1} {2}\n", this._name, this._identifier, this.Value);
            }
        }
    }
}
