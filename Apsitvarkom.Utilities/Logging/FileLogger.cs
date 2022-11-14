using Apsitvarkom.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;

namespace Apsitvarkom.Utilities;

public class FileLogger : ILogger, IDisposable
{
    public event EventHandler<int>? InformationAmountOfBytesPrinted;

    private readonly FileLoggerConfiguration _configuration;
    private readonly BlockingCollection<string> _messagesBlockingQueue = new (new ConcurrentQueue<string>());
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;

    private int bytesPrinted;

    /// <summary>Constructor that is being called by <see cref="FileLoggerProvider"/>.</summary>
    public FileLogger(FileLoggerConfiguration configuration)
    {
        _configuration = configuration;

        // Start a new thread that is solely responsible for writing to a file for this logger
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        Task.Run(WriteLogToFile, _cancellationToken);
    }

    /// <summary>
    /// Scopes are not supported for this logger
    /// </summary>
    public IDisposable BeginScope<TState>(TState state) => NoopDisposable.Instance;

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel, 
        EventId eventId, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        var messageToLog = $"[{DateTime.Now}] [{logLevel}] " + formatter(state, exception);

        if (exception is not null)
            messageToLog += " " + exception.Message + Environment.NewLine + exception.StackTrace;

        messageToLog += Environment.NewLine;

        _messagesBlockingQueue.Add(messageToLog);

        if (_configuration.InformOnEachAmountOfBytes.HasValue)
        {
            Interlocked.Add(ref bytesPrinted, Encoding.ASCII.GetByteCount(messageToLog));
            var bytesPrintedAsOfNow = Interlocked.CompareExchange(ref bytesPrinted, 0, 0);
            if (bytesPrintedAsOfNow > _configuration.InformOnEachAmountOfBytes)
            {
                Interlocked.Exchange(ref bytesPrinted, 0);
                InformationAmountOfBytesPrinted?.Invoke(this, bytesPrintedAsOfNow);
            }
        }
    }

    private void WriteLogToFile()
    {
        if (!File.Exists(_configuration.Path))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_configuration.Path) ?? throw new IOException());
            File.Create(_configuration.Path).Close();
        }

        while (!_cancellationToken.IsCancellationRequested)
        {
            var message = _messagesBlockingQueue.Take();
            File.AppendAllText(_configuration.Path, message);
        }
    }

    /// <summary>
    /// A singleton disposable that does nothing when disposed.
    /// </summary>
    private sealed class NoopDisposable : IDisposable, IAsyncDisposable
    {
        private NoopDisposable(){}

        public void Dispose(){}
        public ValueTask DisposeAsync() => new ValueTask();

        public static NoopDisposable Instance { get; } = new NoopDisposable();
    }
}
