using System;

using log4net;
using log4net.Config;
using log4net.Appender;

namespace InfoHub.Common
{
	/// <summary>
	/// Implementation of ILoggerFactory that produces
	/// Log4NetLogger instances, which themselves use log4net
	/// to perform logging tasks.
	/// </summary>
	public class Log4NetLoggerFactory : ILoggerFactory
	{
		public Log4NetLoggerFactory()
		{
			//In debug builds, add the .NET Trace output log appender
#if DEBUG
			BasicConfigurator.Configure(new log4net.Appender.TraceAppender(new log4net.Layout.SimpleLayout()));
#endif
		}

		#region ILoggerFactory Members

		public ILogger GetLogger(Type forType) {
			return new Log4NetLogger(forType);
		}

		#endregion
	}
}
