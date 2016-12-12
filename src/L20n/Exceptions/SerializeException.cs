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
		/// Thrown when an exception occurs during the serialization of a FTL AST.
		/// </summary>
		public class SerializeException : Exception
		{
			public SerializeException()
				: base()
			{
			}
			
			public SerializeException(string message)
				: base(message)
			{
			}
			
			public SerializeException(string format, params object[] args)
				: base(string.Format(format, args))
			{
			}
			
			public SerializeException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
			
			public SerializeException(string format, Exception innerException, params object[] args)
				: base(string.Format(format, args), innerException)
			{
			}
			
			protected SerializeException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}
	}
}