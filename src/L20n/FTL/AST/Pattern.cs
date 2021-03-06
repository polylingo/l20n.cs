// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a /pattern/.
			/// This can be either a /quoted-pattern/ or /unquoted-pattern/.
			/// More Information: <see cref="L20n.FTL.Parsers.Pattern"/>
			/// </summary>
			public sealed class Pattern : INode
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Pattern"/> class.
				/// </summary>
				public Pattern(bool quoted)
				{
					m_Children = new List<INode>();
					m_Quoted = quoted;
				}
				
				/// <summary>
				/// Adds the child to the list of already existing children.
				/// </summary>
				public void AddChild(INode child)
				{
					m_Children.Add(child);
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					if(m_Quoted)
						writer.Write('"');

					for(int i = 0; i < m_Children.Count; ++i) {
						m_Children[i].Serialize(writer);
					}

					if(m_Quoted)
						writer.Write('"');
				}

				private readonly bool m_Quoted;
				private readonly List<INode> m_Children;
			}
		}
	}
}