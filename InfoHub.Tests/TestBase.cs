using System;
using System.Configuration;

using Spring.Context;

using InfoHub.Common;

namespace InfoHub.Tests
{
	/// <summary>
	/// Abstract base class for all unit tests.   Using a static ctor, 
	/// ensures the Spring framework is initialized before testing commences
	/// </summary>
	public abstract class TestBase
	{
		private static IApplicationContext _ctx;

		/// <summary>
		/// Static ctor, called once when this type is first loaded.
		/// 
		/// Used to initialize Spring.NET framework from config file
		/// before testing commences
		/// </summary>
		static TestBase() {
			//The Spring.Net config handler for spring/context implements
			//IApplicationContext, hence this magical pulling of an IApplicationContext
			//out of an Object.
			_ctx = (IApplicationContext)ConfigurationSettings.GetConfig("spring/context");

			if (_ctx == null) {
				//For some reason, the spring/context section of the config file is not present
				//Tests will likely fail, but throwing an exception here makes nunit & company
				//silently fail, so hope a dire log4net warning does the trick instead
				ILogger logger = new Log4NetLoggerFactory().GetLogger(typeof(TestBase));
				logger.Fatal("Unable to load Spring.NET configuration from 'spring\\context' section of config file.  Some or all tests are likely to fail cataclysmically.");
			}
		}

		/// <summary>
		/// The Spring.NET IApplicationContext to use to resolve named dependencies
		/// </summary>
		internal static IApplicationContext AppContext {
			get {
				return _ctx;
			}
		}
	}
}
