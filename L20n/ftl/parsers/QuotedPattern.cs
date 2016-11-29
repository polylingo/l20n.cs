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
			/// The combinator parser used to parse a quoted-pattern.
			/// A quoted-pattern can be either quoted-text or a placeable.
			/// </summary>
			public static class QuotedPattern
			{
				public static FTL.AST.INode Parse(CharStream cs)
				{
					throw new NotImplementedException();
				}
					
				public static bool PeekAndParse(CharStream cs, out FTL.AST.INode result)
				{
					if(cs.PeekNext() != QUOTE) {
						result = null;
						return false;
					}
						
					result = Parse(cs);
					return true;
				}

				private const char QUOTE = '"';
			}
		}
	}
}