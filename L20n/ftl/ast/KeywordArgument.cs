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
			/// The AST representation for a keyword-argument.
			/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.KeywordArgument"/>
			/// </summary>
			public sealed class KeywordArgument : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.KeywordArgument"/> class.
				/// </summary>
				public KeywordArgument(StringPrimitive identifier, INode quotedPattern)
				{
					m_Identifier = identifier;
					m_QuotedPattern = quotedPattern;
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
					writer.Write(" = \"");
					m_QuotedPattern.Serialize(writer);
					writer.Write('"');
				}
				
				private readonly StringPrimitive m_Identifier;
				private readonly INode m_QuotedPattern;
			}
		}
	}
}