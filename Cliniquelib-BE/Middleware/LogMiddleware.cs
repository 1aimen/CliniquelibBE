using System.Collections.Concurrent;
using System.Text;
using Cliniquelib_BE.Models.Enums.Events;

public interface IAppLogger : ILogger
{
    void LogAppEvent(AppEvents appEvent, string details = null);
    void LogAppError(AppErrors appError, string details = null);
    void LogSysEvent(SysEvents sysEvent, string details = null);
}

public class LogMiddleware : IAppLogger
{
    private readonly string _categoryName;
    private readonly Func<LogLevel, bool> _filter;
    private readonly BlockingCollection<string> _logQueue;
    private readonly AsyncLocal<string> _currentScope = new();

    public LogMiddleware(string categoryName, Func<LogLevel, bool> filter, BlockingCollection<string> logQueue)
    {
        _categoryName = categoryName;
        _filter = filter ?? (_ => true);
        _logQueue = logQueue;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        var oldScope = _currentScope.Value;
        _currentScope.Value = state?.ToString();
        return new ScopeRestorer(() => _currentScope.Value = oldScope);
    }

    public bool IsEnabled(LogLevel logLevel) => _filter(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                            Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var sb = new StringBuilder();
        sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] [{_categoryName}] ");

        if (_currentScope.Value is not null)
            sb.Append($"[Scope: {_currentScope.Value}] ");

        sb.Append(formatter(state, exception));

        if (exception is not null)
        {
            sb.AppendLine();
            sb.Append(exception);
        }

        var message = sb.ToString();

        // Console output
        Console.WriteLine(message);

        // File output via background queue
        _logQueue?.Add(message);
    }

    // Strongly-typed logging for your app
    public void LogAppEvent(AppEvents appEvent, string details = null)
        => Log(LogLevel.Information, default, $"APP EVENT: {appEvent} {details}", null, (s, e) => s.ToString());

    public void LogAppError(AppErrors appError, string details = null)
        => Log(LogLevel.Error, default, $"APP ERROR: {appError} {details}", null, (s, e) => s.ToString());

    public void LogSysEvent(SysEvents sysEvent, string details = null)
        => Log(LogLevel.Information, default, $"SYS EVENT: {sysEvent} {details}", null, (s, e) => s.ToString());

    private class ScopeRestorer : IDisposable
    {
        private readonly Action _restore;
        public ScopeRestorer(Action restore) => _restore = restore;
        public void Dispose() => _restore();
    }
}

public class LogMiddlewareProvider : ILoggerProvider
{
    private readonly Func<LogLevel, bool> _filter;
    private readonly string _filePath;
    private readonly BlockingCollection<string> _logQueue;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundTask;
    private readonly ConcurrentDictionary<string, LogMiddleware> _loggers = new();

    public LogMiddlewareProvider(Func<LogLevel, bool> filter = null, string filePath = null)
    {
        _filter = filter ?? (_ => true);
        _filePath = filePath;

        if (!string.IsNullOrEmpty(filePath))
        {
            _logQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());

            _backgroundTask = Task.Run(async () =>
            {
                try
                {
                    using var writer = new StreamWriter(new FileStream(
                        filePath,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.Read,
                        4096,
                        useAsync: true));

                    foreach (var log in _logQueue.GetConsumingEnumerable(_cts.Token))
                    {
                        await writer.WriteLineAsync(log);
                        await writer.FlushAsync();
                    }
                }
                catch (OperationCanceledException)
                {
                    // expected during shutdown
                }
            }, _cts.Token);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName,
            name => new LogMiddleware(name, _filter, _logQueue));
    }

    public void Dispose()
    {
        if (_logQueue != null)
        {
            _logQueue.CompleteAdding();
            _cts.Cancel();
            try { _backgroundTask?.Wait(2000); } catch { /* swallow */ }
        }
        _cts.Dispose();
    }
}
