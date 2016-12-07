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
			/// The combinator parser used to parse a builtin.
			/// 
			/// [A-Z_.?-]+
			/// </summary>
			public static class Builtin
			{
				public static L20n.FTL.AST.StringPrimitive Parse(CharStream cs)
				{
					string value = cs.ReadWhile(Predicate);
					if(value.Length == 0) {
						throw cs.CreateException(
							"no characters received, while at least one was expected", null);
					}

					return new L20n.FTL.AST.StringPrimitive(value);
				}

				/// <summary>
				/// Determines if the given string primitive is a valid builtin.
				/// </summary>
				public static bool IsValid(string primitive)
				{
					// an empty string is not valid as a keyword
					if(primitive.Length == 0)
						return false;

					// make sure all characters are valid
					for(int i = 0; i < primitive.Length; ++i)
						if(!Predicate(primitive[i]))
							return false;
					
					return true;
				}

				private static bool Predicate(char c)
				{
					return (c >= 'A' && c <= 'Z') || c == '.' || c == '?' || c == '_' || c == '-';
				}
			}
		}
	}
}