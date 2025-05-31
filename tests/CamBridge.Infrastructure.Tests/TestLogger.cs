using Microsoft.Extensions.Logging;

namespace CamBridge.Infrastructure.Tests
{
    /// <summary>
    /// Simple test logger implementation
    /// </summary>
    public class TestLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // Simple console output for testing
            Console.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        }
    }
}
