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
			/// The AST representation for a Comment.
			/// More Information: <see cref="L20n.FTL.Parsers.Comment"/>
			/// </summary>
			public sealed class Comment : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Comment"/> class.
				/// </summary>
				/// <param name="value">Value.</param>
				public Comment(string value)
				{
					m_Values = new List<string>(1);
					m_Values.Add(value);
				}

				/// <summary>
				/// A comment is metadata, and thus optimization does not apply to it.
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
					for(int i = 0; i < m_Values.Count; ++i)
						writer.Writeln("#" + m_Values[i]);
				}

				/// <summary>
				/// Appends the given comment on a new line to the existing comment.
				/// </summary>
				public void Merge(Comment otherComment)
				{
					for(int i = 0; i < otherComment.m_Values.Count; ++i)
						m_Values.Add(otherComment.m_Values[i]);
				}
				
				private List<string> m_Values; // one value per line
			}
		}
	}
}