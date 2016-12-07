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
			/// The parser combinator used to parse all the whitespace.
			/// The resulting output does not get stored.
			/// 
			/// [ \t]*
			/// </summary>
			public static class WhiteSpace
			{
				public static void Parse(CharStream cs)
				{
					cs.SkipWhile(IsWhiteSpace);
				}

				private static bool IsWhiteSpace(char c)
				{
					return c == ' ' || c == '\t';
				}
			}
		}
	}
}