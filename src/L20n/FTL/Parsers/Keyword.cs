// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.IO;
using System.Collections.Generic;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{	
			/// <summary>
			/// The combinator parser used to parse a keyword.
			/// 
			/// [a-zA-Z_.?-] ([a-zA-Z0-9_.?- ]* [a-zA-Z0-9_.?-])?
			/// </summary>
			public static class Keyword
			{
				public static FTL.AST.StringPrimitive Parse(CharStream cs)
				{
					char prefix = cs.ReadNext();
					if(!IsValidPrefix(prefix)) {
						throw cs.CreateException(
							String.Format("{0} is not a valid first character of a keyword", prefix), null);
					}

					string value = prefix + cs.ReadWhile(PostfixPredicate);
					// check if last char is not a space
					if(value[value.Length - 1] == ' ') {
						// it is, so let's discard is,
						// as all whitespace following an identifier is optional
						value = value.Substring(0, value.Length - 1);
					}

					return new FTL.AST.StringPrimitive(value);
				}

				private static bool IsValidPrefix(char c)
				{
					return (c >= 'a' && c <= 'z') ||
						(c >= 'A' && c <= 'Z') ||
						c == '_' || c == '.' || c == '?' || c == '-';
				}

				private static bool PostfixPredicate(char c)
				{
					return (c >= 'a' && c <= 'z') ||
						(c >= 'A' && c <= 'Z') ||
						c == '_' || c == '.' || c == '?' || c == '-' ||
						(c >= '0' && c <= '9') || c == ' ';
				}
			}
		}
	}
}