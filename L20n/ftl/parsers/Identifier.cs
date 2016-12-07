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
			/// The combinator parser used to parse an identifier.
			/// 
			/// [a-zA-Z_.?-] ([a-zA-Z0-9_.?-])*
			/// </summary>
			public static class Identifier
			{
				public static FTL.AST.StringPrimitive Parse(CharStream cs)
				{
					char prefix = cs.ReadNext();
					if(!IsValidPrefix(prefix)) {
						throw cs.CreateException(
							String.Format("{0} is not a valid first character", prefix),
							null, -1);
					}
					
					string value = prefix + cs.ReadWhile(PostfixPredicate);
					return new FTL.AST.StringPrimitive(value);
				}

				public static bool PeekAndParse(CharStream cs, out FTL.AST.INode result)
				{
					if(IsValidPrefix(cs.PeekNext())) {
						result = Parse(cs);
						return true;
					}

					result = null;
					return false;
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
						(c >= '0' && c <= '9');
				}
			}
		}
	}
}