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
					// starts with a newline and optional newline
					NewLine.Parse(cs);
					WhiteSpace.Parse(cs);

					FTL.AST.MemberList memberList = new FTL.AST.MemberList();
					// parse first required member, as we always need at least 1
					memberList.AddMember(Member.Parse(cs));

					char next;
					int bufferPos;

					do {
						next = cs.PeekNext();
						if(CharStream.IsEOF(next) || !CharStream.IsNL(next))
							break;

						bufferPos = cs.Position;
						NewLine.Parse(cs);
						WhiteSpace.Parse(cs);

						if(!Member.Peek(cs)) {
							cs.Rewind(bufferPos);
							break;
						}

						memberList.AddMember(Member.Parse(cs));
					} while(true);

					return memberList;
				}

				public static bool PeekAndParse(CharStream cs, out FTL.AST.MemberList memberList)
				{
					// if next char is not a newline, we can just as well skip already
					char next = cs.PeekNext();
					if(CharStream.IsEOF(next) || !CharStream.IsNL(next)) {
						memberList = null;
						return false;
					}

					// let's keep peeking further, requiring our buffer
					int bufferPos = cs.Position;
					NewLine.Parse(cs);
					WhiteSpace.Parse(cs);
					// we'll always want to rewind no matter what
					bool isMemberList = Member.Peek(cs);
					cs.Rewind(bufferPos);

					if(isMemberList) {
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