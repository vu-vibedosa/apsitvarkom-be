using Apsitvarkom.Utilities;
using Microsoft.Extensions.Logging;

namespace Apsitvarkom.IntegrationTests.Utilities;

public class FileLoggerTests
{
    private string _path;
    private FileLogger _logger;

    [SetUp]
    public void Setup()
    {
        // Files end up relative to the built .exe in temp/
        _path = "temp/" + Guid.NewGuid() + ".log";
        _logger = new FileLogger(new()
        {
            Path = _path
        });
    }

    [Test]
    public async Task CreatesFile()
    {
        _ = await ReadLoggerFile(_path);
        Assert.That(File.Exists(_path), Is.True);
    }

    [Test]
    public async Task WritesOneMessage()
    {
        const string message = "Hello world";

        _logger.LogInformation(message);

        var result = await ReadLoggerFile(_path);

        Assert.That(File.Exists(_path), Is.True);
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0], Does.EndWith(message));
    }

    [Test]
    public async Task WritesMultipleMessages()
    {
        var messages = new string[] { "Hello", "world", "ping", "pong" };

        foreach (var message in messages)
        {
            _logger.LogInformation(message);
        }

        var result = await ReadLoggerFile(_path);

        Assert.That(File.Exists(_path), Is.True);
        Assert.That(result, Has.Length.EqualTo(messages.Length));

        Assert.Multiple(() =>
        {
            for (int i = 0; i < messages.Length; i++)
            {
                Assert.That(result[i], Does.EndWith(messages[i]));
            }
        });
    }

    [Test]
    public async Task WritesMultipleLogLevels()
    {
        const string message = "Hello world";

        var logLevelData = new (LogLevel Level, Action<string> LogAction)[]
        {
            (LogLevel.Debug, x => _logger.LogDebug(x)),
            (LogLevel.Information, x => _logger.LogInformation(x)),
            (LogLevel.Warning, x => _logger.LogWarning(x)),
            (LogLevel.Error, x => _logger.LogError(x))
        };

        foreach (var data in logLevelData)
        {
            data.LogAction.Invoke(message);
        }

        var result = await ReadLoggerFile(_path);

        Assert.That(File.Exists(_path), Is.True);
        Assert.That(result, Has.Length.EqualTo(logLevelData.Length));

        Assert.Multiple(() =>
        {
            for (int i = 0; i < logLevelData.Length; i++)
            {
                Assert.That(result[i], Does.Contain(logLevelData[i].Level.ToString()));
            }
        });
    }

    private static async Task<string[]> ReadLoggerFile(string path)
    {
        // Some delay must be added due to how this logger works (concurrently)
        await Task.Delay(TimeSpan.FromSeconds(1));
        return File.ReadAllLines(path);
    }
}