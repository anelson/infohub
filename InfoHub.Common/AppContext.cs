using System;
using System.Configuration;

using Spring.Context;

namespace InfoHub.Common
{
	/// <summary>
	/// Central singleton object which initializes and provides central access
	/// to the Spring .NET application context
	/// </summary>
	public class AppContext
	{
		static IApplicationContext _ctx;

		//Singleton; no instances allowed
		private AppContext()
		{
		}

		/// <summary>
		/// Initializes Spring from the default app.config location
		/// </summary>
		public static void InitializeAppContext() {
			if (_ctx == null) {
				_ctx = (IApplicationContext)ConfigurationSettings.GetConfig("spring/context");
				if (_ctx== null) {
					throw new ApplicationException("Unable to load Spring .NET configuration settings from spring\\context in the app.config");
				}
			}
		}

		public static IApplicationContext Ctx {
			get {
				return _ctx;
			}
		}

	
	}
}
