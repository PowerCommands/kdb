﻿using System;
using Microsoft.Extensions.Logging;

namespace PainKiller.SerilogExtensions.DomainObjects
{
    public class LoggerBase : ILogger
    {
        private readonly Serilog.ILogger _logger;
        public LoggerBase(Serilog.ILogger logger) { _logger = logger; }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var messageTemplate = $"[{Environment.UserName}]\t{formatter.Invoke(state, exception)}";
            var loglevelEvent = logLevel.ToLogLevel();
            _logger.Write(loglevelEvent, messageTemplate, state, exception);
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            var loglevelEvent = logLevel.ToLogLevel();
            var retVal = _logger.IsEnabled(loglevelEvent);
            return retVal;
        }
        public IDisposable BeginScope<TState>(TState state) { return null; }
    }
}