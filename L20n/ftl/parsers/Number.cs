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
			/// The combinator parser used to parse a number.
			/// </summary>
			public static class Number
			{
				public static FTL.AST.Number Parse(CharStream cs)
				{
					string rawValue = cs.ReadWhile(IsValidDigit);
					if(rawValue.Length == 0) {
						throw cs.CreateException(
							"no <number> digits received, while at least one was expected", null);
					}

					if(cs.PeekNext() == '.') {
						rawValue += cs.ReadNext();
						string fraction = cs.ReadWhile(IsValidDigit);
						if(fraction.Length == 0) {
							throw cs.CreateException(
								"no <number> digits received, while at least one was expected", null);
						}

						rawValue += fraction;
					}

					return new FTL.AST.Number(rawValue);
				}
				
				public static bool PeekAndParse(CharStream cs, out FTL.AST.INode result)
				{
					if(IsValidDigit(cs.PeekNext())) {
						result = Parse(cs);
						return true;
					}
					
					result = null;
					return false;
				}
				
				private static bool IsValidDigit(char c)
				{
					return c >= '0' && c <= '9';
				}
			}
		}
	}
}