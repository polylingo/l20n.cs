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
			/// The AST representation for a /member/.
			/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.Member"/>
			/// </summary>
			public sealed class Member : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Member"/> class.
				/// </summary>
				public Member(INode key, Pattern pattern, bool isDefault)
				{
					m_Key = key;
					m_Pattern = pattern;
					m_IsDefault = isDefault;
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					writer.WriteIndention();
					if(m_IsDefault)
						writer.Write('*');
					writer.Write('[');
					m_Key.Serialize(writer);
					writer.Write("] ");
					m_Pattern.Serialize(writer);
				}
				
				private readonly INode m_Key;
				private readonly Pattern m_Pattern;
				private readonly bool m_IsDefault;
			}
		}
	}
}