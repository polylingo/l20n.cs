// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.IO;
using L20n.Exceptions;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a number.
			/// More Information: <see cref="L20n.FTL.Parsers.Number"/>
			/// </summary>
			public sealed class Number : INode
			{
				public Number(string rawValue)
				{
					try
					{
						m_Value = Convert.ToDouble(rawValue);
					}
					catch(Exception e)
					{
						throw new ParseException("<number> instance could not be created", e);
					}
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					writer.Write(m_Value.ToString());
				}
				
				private readonly double m_Value;
			}
		}
	}
}