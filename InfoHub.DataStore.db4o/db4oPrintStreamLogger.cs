using System;
using System.Collections;
using System.IO;
using System.Reflection;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.DataStore;

using com.db4o;
using j4o.io;

using Spring.Context;

namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// A helper class that puts a db4o PrintStream facade on a logger, 
	/// so db4o log messages can be captured by the info-hub logging infrastructure
	/// 
	/// Unfortunately the log level is not exposed to PrintStream by db4o, so all
	/// log messages will be logged at the info level.  db4oDbEngine will configure
	/// db4o's internal log level to reflect the log settings of the logger passed
	/// to this ctor
	/// </summary>
	internal class db4oPrintStreamLogger : PrintStream
	{
		ILogger _logger;
		String _line;

		public db4oPrintStreamLogger(ILogger logger)
		{
			_logger = logger;
			_line = String.Empty;
		}


		public override void print(object obj) {
			if (obj != null) {
				_line = _line + obj.ToString();
			}
		}
		
		public override void println() {
			println(null);
		}
		
		public override void println(object obj) {
			print(obj);

			if (_line.Length > 0) {
				_logger.Info(_line);
				_line = String.Empty;
			}
		}
	}
}
