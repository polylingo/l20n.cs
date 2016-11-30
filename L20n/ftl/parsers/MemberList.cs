// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{	
			/// <summary>
			/// The combinator parser used to parse a <member-list>.
			/// </summary>
			public static class MemberList
			{
				public static FTL.AST.MemberList Parse(CharStream cs)
				{
					// parse first required member, as we always need at least 1
					WhiteSpace.PeekAndSkip(cs);
					FTL.AST.MemberList memberList = new FTL.AST.MemberList();
					FTL.AST.Member member = Member.Parse(cs);
					memberList.AddMember(member);
					NewLine.Skip(cs);
					WhiteSpace.PeekAndSkip(cs);

					// check for more members...
					while(Member.PeekAndParse(cs, out member)) {
						memberList.AddMember(member);
						NewLine.Skip(cs);
						WhiteSpace.PeekAndSkip(cs);
					}

					return memberList;
				}

				public static bool PeekAndParse(CharStream cs, out FTL.AST.MemberList memberList)
				{
					WhiteSpace.PeekAndSkip(cs);
					if(Member.Peek(cs)) {
						memberList = Parse(cs);
						return true;
					}

					memberList = null;
					return false;
				}
			}
		}
	}
}