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
			/// 
			/// __ '*'? '[' member-key ']' __ pattern
			/// </summary>
			public static class Member
			{
				public static FTL.AST.Member Parse(CharStream cs)
				{
					WhiteSpace.Parse(cs);

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
					WhiteSpace.Parse(cs);
					
					// Parse the actual pattern
					FTL.AST.Pattern pattern = Pattern.Parse(cs);
					
					// and return it all
					return new FTL.AST.Member(key, pattern, isDefault);
				}

				public static bool Peek(CharStream cs)
				{
					char next = cs.PeekNext();

					// if it's a [, we need to make sure we're not dealing with a section
					if(next == '[') {
						int bufferPos = cs.Position;
						cs.SkipNext();
						next = cs.PeekNext();
						cs.Rewind(bufferPos);
						return next != '[';
					}

					// if it's a *, we know for sure it's a section,
					// as no other element starts with it
					if(next == '*')
						return true;

					return false;
				}
			}
		}
	}
}