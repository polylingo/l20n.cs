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
			/// The AST representation for a <member-list>.
			/// </summary>
			public sealed class MemberList : INode
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.MemberList"/> class.
				/// </summary>
				public MemberList()
				{
					m_Members = new List<Member>();
				}
				
				/// <summary>
				/// Adds the member to the list of already existing members.
				/// </summary>
				public void AddMember(Member member)
				{
					m_Members.Add(member);
				}
				
				/// <summary>
				/// TODO
				/// </summary>
				public INode Optimize()
				{
					throw new NotImplementedException();
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					writer.IncreaseIndention();
					for(int i = 0; i < m_Members.Count; ++i) {
						writer.Writeln("");
						m_Members[i].Serialize(writer);
					}
					writer.DecreaseIndention();
				}

				private readonly List<Member> m_Members;
			}
		}
	}
}