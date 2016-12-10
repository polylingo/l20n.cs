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
			/// represents the AST element for an entity, AKA <message>
			/// </summary>
			public sealed class Entity : INode
			{	
				public Entity(StringPrimitive identifier, Pattern pattern, MemberList memberList)
				{
					m_Identifier = identifier;
					m_Pattern = pattern;
					m_MemberList = memberList;
				}

				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					// write the comment if one is attached to this message
					if(m_Comment != null)
						m_Comment.Serialize(writer);

					// write the actual content
					m_Identifier.Serialize(writer);
					writer.Write(" = ");
					if(m_Pattern != null)
						m_Pattern.Serialize(writer);
					if(m_MemberList != null)
						m_MemberList.Serialize(writer);
					writer.Writeln("");
				}

				/// <summary>
				/// Attaches the given comment to this message.
				/// </summary>
				public void AttachComment(Comment comment)
				{
					m_Comment = comment;
				}

				private Comment m_Comment;
				private readonly StringPrimitive m_Identifier;
				private readonly Pattern m_Pattern;
				private readonly MemberList m_MemberList;
			}
		}
	}
}