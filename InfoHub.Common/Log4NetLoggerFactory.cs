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
			//Load the config from the app.config file
			DOMConfigurator.Configure();
		}

		#region ILoggerFactory Members

		public ILogger GetLogger(Type forType) {
			return new Log4NetLogger(forType);
		}

		#endregion
	}
}
