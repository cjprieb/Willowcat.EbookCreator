using Microsoft.Extensions.Logging;
using System;

namespace Willowcat.EbookCreator.Utilities
{
    public class DefaultConsoleLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState _) => new SimpleDisposableObject();

        public bool IsEnabled(LogLevel _) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            Console.WriteLine($"{logLevel} {eventId} {message}");
        }
    }

    class SimpleDisposableObject : IDisposable
    {
        public void Dispose() { }
    }
}
