// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for any StringPrimitive.
			/// Used for <builtin>, <keyword>, <identifier> and <unquotedText>
			/// </summary>
			public sealed class StringPrimitive : INode
			{
				/// <summary>
				/// Gets the internal string as a constant.
				/// </summary>
				public string Value
				{
					get { return m_Value; }
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.StringPrimitive"/> class.
				/// </summary>
				public StringPrimitive(string value)
				{
					Console.WriteLine(value);
					m_Value = value;
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
				public void Serialize(Writer writer)
				{
					writer.Write(m_Value);
				}
				
				private readonly string m_Value;
			}
		}
	}
}