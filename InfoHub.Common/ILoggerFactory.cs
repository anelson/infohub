using System;

namespace InfoHub.Common
{
	/// <summary>
	/// Interface for a factory class that creates ILogger instances
	/// on request.
	/// </summary>
	public interface ILoggerFactory
	{
		/// <summary>
		/// Builds and returns an ILogger instance for logging messages from an
		/// instance of a given type.  This type information is used to filter
		/// and group log messages, thus each type must use a separate logger, and
		/// in practice each instance of a type will maintain a separate logger instance.
		/// </summary>
		/// <param name="forType">The type for which this logger is being requested</param>
		/// <returns></returns>
		ILogger GetLogger(Type forType);
	}
}
