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
			/// The AST representation for <block-text>.
			/// More Information: <see cref="L20n.FTL.Parsers.BlockText"/>
			/// </summary>
			public sealed class BlockText : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.BlockText"/> class.
				/// </summary>
				public BlockText()
				{
					m_Lines = new List<INode>();
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					// write all lines, except last one
					writer.IncreaseIndention();
					for(int i = 0; i < m_Lines.Count; ++i) {
						writer.Writeln("");
						writer.WriteIndention();
						writer.Write("| ");
						m_Lines[i].Serialize(writer);
					}
					writer.DecreaseIndention();
				}
				
				/// <summary>
				/// Appends the given comment on a new line to the existing comment.
				/// </summary>
				public void AddLine(INode line)
				{
					m_Lines.Add(line);
				}
				
				private List<INode> m_Lines; // one value per line
			}
		}
	}
}