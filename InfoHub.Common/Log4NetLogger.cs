using System;

using log4net;

namespace InfoHub.Common
{
	/// <summary>
	/// Implementation of ILogger that creates and logs to a log4net
	/// ILog instance named with the type name of the caller.
	/// </summary>
	internal class Log4NetLogger : ILogger
	{
		private ILog _log;

		/// <summary>
		/// Creates a new instance, using the specified type as the source
		/// for log messages.
		/// </summary>
		/// <param name="forType"></param>
		public Log4NetLogger(Type forType) {
			_log = log4net.LogManager.GetLogger(forType);
		}

		#region ILogger Members

		public bool IsDebugEnabled {
			get {
				return _log.IsDebugEnabled;
			}
		}

		public bool IsInfoEnabled {
			get {
				return _log.IsInfoEnabled;
			}
		}

		public bool IsWarnEnabled {
			get {
				return _log.IsWarnEnabled;
			}
		}

		public bool IsErrorEnabled {
			get {
				return _log.IsErrorEnabled;
			}
		}

		public bool IsFatalEnabled {
			get {
				return _log.IsFatalEnabled;
			}
		}

		public void Debug(String msg, params Object[] args) {
			_log.Debug(String.Format(msg, args));
		}

		public void Info(String msg, params Object[] args) {
			_log.Info(String.Format(msg, args));
		}

		public void Warn(String msg, params Object[] args) {
			_log.Warn(String.Format(msg, args));
		}

		public void Error(String msg, params Object[] args) {
			_log.Error(String.Format(msg, args));
		}

		public void Fatal(String msg, params Object[] args) {
			_log.Fatal(String.Format(msg, args));
		}

		#endregion
	}
}
