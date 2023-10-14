using Microsoft.Extensions.Logging;
using System.Text;

namespace YDotNet.Server.Redis.Internal;

internal sealed class LoggerTextWriter : TextWriter
{
    private readonly ILogger log;

    public LoggerTextWriter(ILogger log)
    {
        this.log = log;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
    }

    public override void WriteLine(string? value)
    {
        if (log.IsEnabled(LogLevel.Debug))
        {
#pragma warning disable CA2254 // Template should be a static expression
            log.LogDebug(new EventId(100, "RedisConnectionLog"), value);
#pragma warning restore CA2254 // Template should be a static expression
        }
    }
}
