using System;

namespace CleanDds.Infrastructure.Logging.Internal;

public struct LogMessage
{
    public DateTimeOffset Timestamp { get; set; }
    public string Message { get; set; }
}
