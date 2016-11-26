// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		/// <summary>
		/// A static class with the sole goal of creating <c>StreamReader</c> instances.
		/// The reason why this is abstracted away in this class, is so that you can override the actual
		/// creation method in case you require a custom method
		/// that goes beyond the default C# StreamReader constructor.
		/// </summary>
		public static class StreamReaderFactory
		{
			/// <summary>
			/// If not null, it will be used as the new creation method to make StreamReader instances.
			/// Will reset to the default creation logic in case the given <c>callback</c> is <c>null</c>.
			/// </summary>
			public static void SetCallback(DelegateType callback)
			{
				s_Delegate = callback;
			}
				
			/// <summary>
			/// Return a new StreamReader instance for the <c>UTF8</c> resource at the given <c>path</c>.
			/// </summary>
			public static StreamReader Create(string path)
			{
				if(s_Delegate == null)
					return new StreamReader(path, System.Text.Encoding.UTF8, false);
				return s_Delegate(path, System.Text.Encoding.UTF8, false);
			}
				
			public delegate StreamReader DelegateType
					(string path,System.Text.Encoding encoding,bool detectEncoding);
				
			private static DelegateType s_Delegate = null;
		}
	}
}