using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CarRentingSystem2025.Services
{
    public interface IPerformanceService
    {
        IDisposable MeasureOperation(string operationName);
        void LogPerformance(string operationName, TimeSpan duration);
        Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation);
        T Measure<T>(string operationName, Func<T> operation);
    }

    public class PerformanceService : IPerformanceService
    {
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(ILogger<PerformanceService> logger)
        {
            _logger = logger;
        }

        public IDisposable MeasureOperation(string operationName)
        {
            return new PerformanceOperation(_logger, operationName);
        }

        public void LogPerformance(string operationName, TimeSpan duration)
        {
            _logger.LogInformation("Performance: {OperationName} took {Duration}ms", 
                operationName, duration.TotalMilliseconds);
        }

        public async Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await operation();
                return result;
            }
            finally
            {
                stopwatch.Stop();
                LogPerformance(operationName, stopwatch.Elapsed);
            }
        }

        public T Measure<T>(string operationName, Func<T> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = operation();
                return result;
            }
            finally
            {
                stopwatch.Stop();
                LogPerformance(operationName, stopwatch.Elapsed);
            }
        }

        private class PerformanceOperation : IDisposable
        {
            private readonly ILogger _logger;
            private readonly string _operationName;
            private readonly Stopwatch _stopwatch;

            public PerformanceOperation(ILogger logger, string operationName)
            {
                _logger = logger;
                _operationName = operationName;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                _logger.LogInformation("Performance: {OperationName} took {Duration}ms", 
                    _operationName, _stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}


