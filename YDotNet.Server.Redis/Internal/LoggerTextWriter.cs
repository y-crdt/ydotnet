using System.Text;
using Microsoft.Extensions.Logging;

namespace YDotNet.Server.Redis.Internal;

internal sealed class LoggerTextWriter(ILogger log) : TextWriter
{
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
