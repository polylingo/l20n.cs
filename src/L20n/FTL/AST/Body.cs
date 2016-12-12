// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20n.IO;
using L20n.Exceptions;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// Represents the Root AST Element, /body/, containing all /entries/.
			/// More Information: <see cref="L20n.FTL.Parsers.Parser"/>
			/// </summary>
			public sealed class Body : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Body"/> class.
				/// </summary>
				public Body(FTL.Parsers.Context ctx)
				{
					// define EntryAdder, for games (partial) its very simple logic,
					// as we only ever expect entities as entries.
					if(ctx.ASTType == FTL.Parsers.Context.ASTTypes.Partial)
						m_EntryAdder = AddEntryPartial;
					else
						m_EntryAdder = AddEntryFull;

					m_Entries = new List<INode>();
				}

				/// <summary>
				/// Writes its content and the content of its children.
				/// In other words, this writes the entire FTL document.
				/// </summary>
				public void Serialize(Writer writer)
				{
					for(int i = 0; i < m_Entries.Count; ++i) {
						m_Entries[i].Serialize(writer);
					}
				}

				/// <summary>
				/// Adds the entry to the body.
				/// </summary>
				public void AddEntry(INode entry)
				{
					m_EntryAdder(entry);
				}

				private delegate void EntryAdder(INode entry);

				private void AddEntryPartial(INode entry)
				{
					m_Entries.Add(entry);
				}
				
				private void AddEntryFull(INode entry)
				{
					if(m_Entries.Count == 0) {
						m_Entries.Add(entry);
						return;
					}

					int lastPosition = m_Entries.Count - 1;
					Comment comment = m_Entries[lastPosition] as Comment;
					if(comment == null) {
						m_Entries.Add(entry);
						return;
					}

					Comment newComment = entry as Comment;
					// merge comments together
					if(newComment != null) {
						comment.Merge(newComment);
						return;
					}

					// add comment to section
					Section section = entry as Section;
					if(section != null) {
						section.AttachComment(comment);
						m_Entries.RemoveAt(lastPosition);
						m_Entries.Add(section);
						return;
					}

					// add comment to message
					Entity message = entry as Entity;
					if(message != null) {
						message.AttachComment(comment);
						m_Entries.RemoveAt(lastPosition);
						m_Entries.Add(message);
						return;
					}

					throw new ParseException("a section or message was expexted," +
					    " please report this as a bug at https://github.com/polylingo/l20n.cs/issues," +
						" as this situation should never occur." +
						" Attach the locale files you used while this bug occured to your bug report.");
				}

				private EntryAdder m_EntryAdder;
				private List<INode> m_Entries;
			}
		}
	}
}