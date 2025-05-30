using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CamBridge.Infrastructure.Tests
{
    public class TestLogger<T> : ILogger<T>
    {
        public List<string> LoggedMessages { get; } = new List<string>();

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            LoggedMessages.Add($"[{logLevel}] {message}");
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }
}
