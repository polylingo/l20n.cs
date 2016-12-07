// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.Exceptions;
using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{	
			/// <summary>
			/// The parser combinator used to parse all the newlines.
			/// The resulting output does not get stored.
			/// 
			/// [\r\n]+
			/// </summary>
			public static class NewLine
			{
				public static void Parse(CharStream cs)
				{
					if(cs.SkipWhile(IsNewLine) <= 0)
						throw cs.CreateException(
							"at least one newline character is required", null);
				}
				
				private static bool IsNewLine(char c)
				{
					return c == '\r' || c == '\n';
				}
			}
		}
	}
}