using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;

namespace InfoHub.Common
{
	/// <summary>
	/// Helper class that wraps the ILogger interface and provides
	/// easier string formatting and loading of localized string
	/// resources
	/// </summary>
	public class LoggerHelper {
		private ILogger _logger;
		private Assembly _resAsm;
		private ResourceManager _resMgr;

		/// <summary>
		/// Creates a new LoggerHelper instance which writes log messages to
		/// a given ILog instance.
		/// 
		/// Will use the assembly of the caller to load string resources.
		/// Expects the string resources to be located in a string bundle
		/// named with the assembly name, a dot (.), and
		/// stringConstantsBundleName.
		/// </summary>
		/// <param name="logger"></param>
		public LoggerHelper(ILogger logger, String stringConstantsBundleName) : this(logger, Assembly.GetCallingAssembly(), stringConstantsBundleName)  {
		}

		/// <summary>
		/// Creates a new LoggerHelper instance which writes log messages to
		/// a given ILog instance.
		/// 
		/// Will load string resources from resAsm
		/// </summary>
		public LoggerHelper(ILogger logger, Assembly resAsm, String stringConstantsBundleName) {
			System.Diagnostics.Debug.Assert(logger != null);

			_logger = logger;
			_resAsm = resAsm;
			_resMgr = new ResourceManager(_resAsm.GetName().Name + "." + stringConstantsBundleName, _resAsm);
		}

		public ILogger Logger {
			get {
				return _logger;
			}
		}

		/// <summary>
		/// Writes a debug message to the log, using a named string resource 
		/// to compute the message, expanding placeholders with the additional arguments
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="args"></param>
		public void Debug(String resourceName, params Object[] args) {
			_logger.Debug(GetResourceString(resourceName), args);
		}

		/// <summary>
		/// Writes a info message to the log, using a named string resource 
		/// to compute the message, expanding placeholders with the additional arguments
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="args"></param>
		public void Info(String resourceName, params Object[] args) {
			_logger.Info(GetResourceString(resourceName), args);
		}

		/// <summary>
		/// Writes a warn message to the log, using a named string resource 
		/// to compute the message, expanding placeholders with the additional arguments
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="args"></param>
		public void Warn(String resourceName, params Object[] args) {
			_logger.Warn(GetResourceString(resourceName), args);
		}

		/// <summary>
		/// Writes a error message to the log, using a named string resource 
		/// to compute the message, expanding placeholders with the additional arguments
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="args"></param>
		public void Error(String resourceName, params Object[] args) {
			_logger.Error(GetResourceString(resourceName), args);
		}

		/// <summary>
		/// Writes a fatal message to the log, using a named string resource 
		/// to compute the message, expanding placeholders with the additional arguments
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="args"></param>
		public void Fatal(String resourceName, params Object[] args) {
			_logger.Fatal(GetResourceString(resourceName), args);
		}

		/// <summary>
		/// Loads a named string resource from the resource assembly
		/// </summary>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		private String GetResourceString(String resourceName) {
			String resStringVal = _resMgr.GetString(resourceName);
			if (resStringVal == null) {
				//Resource name isn't found
				System.Diagnostics.Debug.Assert(false, String.Format("No string resource named '{0}'", resourceName));
				return String.Format("No string resource named '{0}'", resourceName);
			}

			return resStringVal;
		}
	}
}
