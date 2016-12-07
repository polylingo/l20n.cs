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
			/// The combinator parser used to parse a member-expression.
			/// 
			/// identifier '[' keyword ']'
			/// </summary>
			public static class MemberExpression
			{
				public static FTL.AST.MemberExpression Parse(CharStream cs, FTL.AST.StringPrimitive identifier)
				{
					cs.SkipCharacter(PREFIX);
					FTL.AST.StringPrimitive keyword = Keyword.Parse(cs);
					cs.SkipCharacter(POSTFIX);
					
					return new FTL.AST.MemberExpression(identifier, keyword);
				}
				
				public static bool Peek(CharStream cs)
				{
					return cs.PeekNext() == PREFIX;
				}

				private const char PREFIX = '[';
				private const char POSTFIX = ']';
			}
		}
	}
}