using System;

namespace Joserras.Commons.Logging
{
	public static class LoggingExtensions
	{
    public static void Debug(this ILog logger, string message)
    {
      logger.Log( new LogEntry( LoggingEventType.Debug, message ) );
    }

    public static void Info(this ILog logger, string message)
    {
      logger.Log( new LogEntry( LoggingEventType.Information, message ) );
    }

    public static void Warn(this ILog logger, string message)
    {
      logger.Log( new LogEntry( LoggingEventType.Warning, message ) );
    }

    public static void Error(this ILog logger, Exception ex)
    {
      logger.Log( new LogEntry( LoggingEventType.Error, ex.Message, ex ) );
    }

    public static void Fatal(this ILog logger, Exception ex)
    {
      logger.Log( new LogEntry( LoggingEventType.Fatal, ex.Message, ex ) );
    }

  }

}