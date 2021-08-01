using System;

namespace Joserras.Commons.Logging
{
	public interface ILog
	{
		void Log(LogEntry entry);
	}

  public enum LoggingEventType { Debug, Information, Warning, Error, Fatal };

  // Immutable DTO that contains the log information.
  public class LogEntry
  {
    public LoggingEventType Severity { get; }
    public string Message { get; }
    public Exception Exception { get; }

    public LogEntry(
        LoggingEventType severity, string message, Exception exception = null)
    {
      if (message == null) throw new ArgumentNullException( nameof( Message ) );
      if (message == string.Empty)
			{
        throw new ArgumentException( "empty", nameof( Message ) );
      }

      Severity = severity;
      Message = message;
      Exception = exception;
    }

  }

}