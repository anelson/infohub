using System;

namespace InfoHub.Common
{
	/// <summary>
	/// Interface for a logging object used by other components to write
	/// log information for debugging and monitoring purposes.
	/// </summary>
	public interface ILogger {
		bool IsDebugEnabled{get;}
		bool IsInfoEnabled{get;}
		bool IsWarnEnabled{get;}
		bool IsErrorEnabled{get;}
		bool IsFatalEnabled{get;}

		void Debug(String msg, params Object[] args);
		void Info(String msg, params Object[] args);
		void Warn(String msg, params Object[] args);
		void Error(String msg, params Object[] args);
		void Fatal(String msg, params Object[] args);
	}
}
