// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20n
{
	namespace IO
	{
		/// <summary>
		/// A simple static class to Log warnings.
		/// The logic used to actually log the message,
		/// can be overriden to use the used environment rather than the System calls.
		/// </summary>
		public static class Logger
		{
			/// <summary>
			/// Defines the minimum level to log, the higher the value,
			/// the more types of logging will come through.
			/// </summary>
			public enum LogLevel : byte
			{
				Error = 1,
				Warning = 2,
				Info = 4,
			}

			/// <summary>
			/// Gets or sets the level of the logger.
			/// </summary>
			public static LogLevel Level {
				get { return s_Level; }
				set { s_Level = value; }
			}

			/// <summary>
			/// The Current Locale used by L20n, used to provide extra context to the logs.
			/// </summary>
			public static string CurrentLocale { get; set; }

			/// <summary>
			/// Logs an info message, when the set LogLevel allows it.
			/// </summary>
			public static void Info(string format, params object[] argv)
			{
				if(Level >= LogLevel.Info) {
					s_STDOut(
						String.Format("[L20n][{0}][INFO] {1}", CurrentLocale, format),
						argv);
				}
			}
			
			/// <summary>
			/// Logs an warning message, when the set LogLevel allows it.
			/// </summary>
			public static void Warning(string format, params object[] argv)
			{
				if(Level >= LogLevel.Warning) {
					s_STDOut(
						String.Format("[L20n][{0}][WARNING] {1}", CurrentLocale, format),
						argv);
				}
			}
			
			/// <summary>
			/// Logs an error message, when the set LogLevel allows it.
			/// </summary>
			public static void Error(string format, params object[] argv)
			{
				if(Level >= LogLevel.Error) {
					s_STDErr(
					String.Format("[L20n][{0}][ERROR] {1}", CurrentLocale, format),
					argv);
				}
			}
			
			/// <summary>
			/// When cb is null, the warning will be logged to the stdout,
			/// otherwise the error will be given to the callback.
			/// </summary>
			public static void SetSTDOut(LogDelegate cb)
			{
				if(cb == null)
					cb = Console.WriteLine;
				s_STDOut = cb;
			}
			
			/// <summary>
			/// When cb is null, the error will be logged to the stderr,
			/// otherwise the error will be given to the callback.
			/// </summary>
			public static void SetSTDErr(LogDelegate cb)
			{
				if(cb == null)
					cb = Console.Error.WriteLine;
				s_STDErr = cb;
			}
			
			public delegate void LogDelegate(string msg,params object[] argv);
			
			private static LogDelegate s_STDOut = Console.WriteLine;
			private static LogDelegate s_STDErr = Console.Error.WriteLine;
			private static LogLevel s_Level = LogLevel.Info;
		}
	}
}