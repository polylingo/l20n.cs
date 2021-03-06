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
			/// The AST representation for a /section/.
			/// More Information: <see cref="L20n.FTL.Parsers.Section"/>
			/// </summary>
			public sealed class Section : INode
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Section"/> class.
				/// </summary>
				public Section(StringPrimitive keyword)
				{
					m_Keyword = keyword;
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					// write the comment if one is attached
					if(m_Comment != null)
						m_Comment.Serialize(writer);

					// write the actual section
					writer.Write("[[ ");
					m_Keyword.Serialize(writer);
					writer.Writeln(" ]]");
				}

				/// <summary>
				/// Attaches the given comment to this section.
				/// </summary>
				public void AttachComment(Comment comment)
				{
					m_Comment = comment;
				}
				
				private readonly StringPrimitive m_Keyword;
				private Comment m_Comment;
			}
		}
	}
}