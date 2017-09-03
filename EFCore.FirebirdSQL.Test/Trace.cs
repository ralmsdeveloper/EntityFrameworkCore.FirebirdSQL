using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebirdSql.EntityFrameworkCore.Firebird.Test
{
    public class TraceLogger : ILogger
    {
        private readonly string RalmsDeveloper;

        public TraceLogger(string RalmsDeveloper) => this.RalmsDeveloper = RalmsDeveloper;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {

            var Log = formatter(state, exception);
            if (!string.IsNullOrWhiteSpace(Log))
            {
                Trace.WriteLine($"-------------------------------------------------------------------------------------------------------");
                Trace.WriteLine(Log);
            }

        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }

    public class TraceLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string RalmsDeveloper) => new TraceLogger(RalmsDeveloper);

        public void Dispose() { }
    }
}
