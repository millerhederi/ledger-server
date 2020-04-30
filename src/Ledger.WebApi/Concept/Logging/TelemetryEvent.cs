using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ledger.WebApi.Concept.Logging
{
    public class TelemetryEvent : IDisposable
    {
        private SortedDictionary<string, object> _capturedTelemetryData;
        private readonly ConcurrentDictionary<string, TimeSpan> _durations = new ConcurrentDictionary<string, TimeSpan>();
        private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentBag<Exception> _exceptions = new ConcurrentBag<Exception>();
        private readonly IDisposable _timer;
        private bool _disposed;

        public TelemetryEvent(string name)
        {
            this["Name"] = name;
            _timer = TrackElapsedTime("Duration");
        }

        public object this[string key]
        {
            get => _data[key];
            set => _data[key] = value;
        }

        public IDisposable TrackElapsedTime(string name)
        {
            return new TelemetryEventTimer(TrackDuration);

            void TrackDuration(TimeSpan duration)
            {
                if (!_durations.TryAdd(name, duration))
                {
                    throw new Exception($"Unable to track the elapsed time for the timer '{name}'.");
                }
            }
        }

        public void AddException(Exception e)
        {
            _exceptions.Add(e);
        }

        public Task<string> SerializeAsync()
        {
            if (_capturedTelemetryData == null)
            {
                throw new InvalidOperationException("Must dispose of the telemetry event before attempting to serialize.");
            }

            return Task.FromResult(JsonSerializer.Serialize(_capturedTelemetryData));
        }

        private void CaptureTelemetryData()
        {
            _capturedTelemetryData = new SortedDictionary<string, object>(_data);

            CaptureExceptions();
            CaptureTimings();

            void CaptureExceptions()
            {
                if (_exceptions.Count == 0)
                {
                    return;
                }

                var exception = _exceptions.Count == 1 ? _exceptions.Single() : new AggregateException(_exceptions);

                _capturedTelemetryData.Add("Exception", new { Data = exception.ToString(), Type = exception.GetType().Name });
            }

            void CaptureTimings()
            {
                var timings = new SortedDictionary<string, int>();

                foreach (var (name, timespan) in _durations)
                {
                    // Avoid adding timings data to the telemetry when the duration is insignificant
                    if (timespan.Milliseconds > 2)
                    {
                        timings.Add(name, timespan.Milliseconds);
                    }
                }

                _capturedTelemetryData.Add("Timings", timings);
            }
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _timer.Dispose();

            CaptureTelemetryData();

            _disposed = true;
        }

        private class TelemetryEventTimer : IDisposable
        {
            private readonly Stopwatch _stopwatch;
            private readonly Action<TimeSpan> _trackDurationFn;

            public TelemetryEventTimer(Action<TimeSpan> trackDurationFn)
            {
                _trackDurationFn = trackDurationFn;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _trackDurationFn(_stopwatch.Elapsed);
            }
        }
    }
}