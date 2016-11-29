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
			/// The AST representation for a member-expression.
			/// More Information: <see cref="L20n.FTL.Parsers.MemberExpression"/>
			/// </summary>
			public sealed class MemberExpression : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.MemberExpression"/> class.
				/// </summary>
				public MemberExpression(StringPrimitive identifier, StringPrimitive keyword)
				{
					m_Identifier = identifier;
					m_Keyword = keyword;
				}

				/// <summary>
				/// Returns the most optimized form of itself.
				/// </summary>
				public INode Optimize()
				{
					throw new NotImplementedException();
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(TextWriter writer)
				{
					m_Identifier.Serialize(writer);
					writer.Write('[');
					m_Keyword.Serialize(writer);
					writer.Write(']');
				}
					
				private readonly StringPrimitive m_Identifier;
				private readonly StringPrimitive m_Keyword;
			}
		}
	}
}