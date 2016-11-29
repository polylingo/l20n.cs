// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;

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
				/// Returns itself, as it can't be optimized any further.
				/// </summary>
				public INode Optimize()
				{
					return this;
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(TextWriter writer)
				{
					writer.Write(m_Value);
				}
				
				private readonly double m_Value;
			}
		}
	}
}