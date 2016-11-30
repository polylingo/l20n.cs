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
			/// The combinator parser used to parse a member.
			/// A member consists out of a member-key and a pattern.
			/// </summary>
			public static class Member
			{
				public static FTL.AST.Member Parse(CharStream cs)
				{
					bool isDefault = false;

					if(cs.PeekNext() == '*') {
						cs.SkipNext();
						isDefault = true;
					}
					
					// Parse the MemberKey
					cs.SkipCharacter('[');
					FTL.AST.INode key = Memberkey.Parse(cs);
					cs.SkipCharacter(']');
					
					// skip optional space
					WhiteSpace.PeekAndSkip(cs);
					
					// Parse the actual pattern
					FTL.AST.Pattern pattern = Pattern.Parse(cs);
					
					// and return it all
					return new FTL.AST.Member(key, pattern, isDefault);
				}

				public static bool Peek(CharStream cs)
				{
					char next = cs.PeekNext();
					return next == '*' || next == '[';
				}

				public static bool PeekAndParse(CharStream cs, out FTL.AST.Member member)
				{
					if(Peek(cs)) {
						member = Parse(cs);
						return true;
					}

					member = null;
					return false;
				}
			}
		}
	}
}