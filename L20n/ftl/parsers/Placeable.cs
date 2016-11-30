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
			/// The combinator parser used to parse a placeable.
			/// A placeable contains one or more (select-)expressions.
			/// </summary>
			public static class Placeable
			{
				public static bool PeekAndParse(CharStream cs, out FTL.AST.INode result)
				{
					if(cs.PeekNext() != PREFIX) {
						result = null;
						return false;
					}

					// skip prefix and optional space
					cs.SkipCharacter(PREFIX);

					FTL.AST.Placeable placeable = new FTL.AST.Placeable();
					// parse all placeable-expressions
					do {
						WhiteSpace.PeekAndSkip(cs);
						placeable.AddExpression(PlaceableExpression.Parse(cs));
						WhiteSpace.PeekAndSkip(cs);
						if(cs.PeekNext() != ',') {
							break;
						}

						// keep going, until we have no more commas
						cs.SkipNext();
					} while(true);

					// skip optional space and postfix
					WhiteSpace.PeekAndSkip(cs);
					cs.SkipCharacter(POSTFIX);

					result = placeable;
					return true;
				}
				
				private const char PREFIX = '{';
				private const char POSTFIX = '}';
			}
		}
	}
}