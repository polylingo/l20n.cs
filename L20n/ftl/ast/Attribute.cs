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
			/// The AST representation for an attribute.
			/// This is not in the official FTL Syntax,
			/// and is used to store the combination of an identifier and a keyword.
			/// More Information: <see cref="L20n.FTL.Parsers.MemberKey"/>
			/// </summary>
			public sealed class Attribute : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Attribute"/> class.
				/// </summary>
				public Attribute(StringPrimitive identifier, StringPrimitive keyword)
				{
					m_Identifier = identifier;
					m_Keyword = keyword;
				}
				
				/// <summary>
				/// Returns the most optimized form of itself.
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
					m_Identifier.Serialize(writer);
					writer.Write('/');
					m_Keyword.Serialize(writer);
				}
				
				private readonly StringPrimitive m_Identifier;
				private readonly StringPrimitive m_Keyword;
			}
		}
	}
}