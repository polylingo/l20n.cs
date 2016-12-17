// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Runtime.Serialization;

namespace L20n
{
	namespace Exceptions
	{
		[Serializable]
		/// <summary>
		/// Thrown when an exception occurs because of input given by the user.
		/// </summary>
		public class InputException : Exception
		{
			public InputException()
				: base()
			{
			}
			
			public InputException(string message)
				: base(message)
			{
			}
			
			public InputException(string format, params object[] args)
				: base(string.Format(format, args))
			{
			}
			
			public InputException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
			
			public InputException(string format, Exception innerException, params object[] args)
				: base(string.Format(format, args), innerException)
			{
			}
			
			protected InputException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}
	}
}