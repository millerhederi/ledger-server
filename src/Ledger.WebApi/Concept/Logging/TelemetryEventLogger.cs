using System;
using System.Threading.Tasks;

namespace Ledger.WebApi.Concept.Logging
{
    public interface ITelemetryEventLogger
    {
        Task LogAsync(TelemetryEvent telemetryEvent);
    }

    public class TelemetryEventLogger : ITelemetryEventLogger
    {
        public async Task LogAsync(TelemetryEvent telemetryEvent)
        {
            ((IDisposable) telemetryEvent).Dispose();
            await Console.Out.WriteLineAsync(await telemetryEvent.SerializeAsync());
        }
    }
}