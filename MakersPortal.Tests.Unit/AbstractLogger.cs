﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace MakersPortal.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    public abstract class AbstractLogger<T> : ILogger<T> where T : class
    {
        public IDisposable BeginScope<TState>(TState state)
            => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => Log(logLevel, exception, formatter(state, exception));

        public abstract void Log(LogLevel logLevel, Exception ex, string information);
    }
}