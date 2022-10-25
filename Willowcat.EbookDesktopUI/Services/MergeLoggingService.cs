using Microsoft.Extensions.Logging;
using System;

namespace Willowcat.EbookDesktopUI.Services
{
    public class MergeLoggingService<T> : ILogger<T>
    {
        private IProgress<LogItem> _Progress = null;

        public IDisposable BeginScope<TState>(TState _) => new SimpleDisposableObject();

        public MergeLoggingService(IProgress<LogItem> progress) 
        {
            _Progress = progress;
        }

        public bool IsEnabled(LogLevel _) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            _Progress.Report(new LogItem()
            {
                LogLevel = logLevel,
                Message = message
            });
        }
    }

    public class LogItem
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
    }

    class SimpleDisposableObject : IDisposable
    {
        public void Dispose() { }
    }
}
