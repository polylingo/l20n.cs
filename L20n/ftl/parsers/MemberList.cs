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
			/// 
			/// NL member+
			/// </summary>
			public static class MemberList
			{
				public static FTL.AST.MemberList Parse(CharStream cs)
				{
					// starts with a newline
					NewLine.Parse(cs);

					FTL.AST.MemberList memberList = new FTL.AST.MemberList();
					// parse first required member, as we always need at least 1
					FTL.AST.Member member = Member.Parse(cs);

					char next;
					while(member != null) {
						memberList.AddMember(member);
						next = cs.PeekNext();
						if(next == CharStream.EOF || !CharStream.IsNL(next))
							break;

						cs.StartBuffering();
						//NewLine.Parse(cs);
						cs.StopBuffering();

						if(!Member.Peek(cs)) {
							member = null;
						} else {
							cs.FlushBuffer();
							member = Member.Parse(cs);
						}
					}

					return memberList;
				}

				public static bool PeekAndParse(CharStream cs, out FTL.AST.MemberList memberList)
				{
					// if next char is not a newline, we can just as well skip already
					char next = cs.PeekNext();
					if(next == CharStream.EOF || !CharStream.IsNL(next)) {
						memberList = null;
						return false;
					}

					// let's keep peeking further, requiring our buffer
					cs.StartBuffering();
					NewLine.Parse(cs);
					cs.StopBuffering();

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