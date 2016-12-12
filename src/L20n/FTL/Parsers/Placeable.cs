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
			/// 
			/// '{' __ placeable-list __ '}';
			/// placeable-list ::= placeable-expression (__ ',' __ NL? __ placeable-list)?;
			/// </summary>
			public static class Placeable
			{
				public static bool PeekAndParse(CharStream cs, out FTL.AST.INode result)
				{
					if(!Peek(cs)) {
						result = null;
						return false;
					}

					// skip prefix and optional space
					cs.SkipCharacter(PREFIX);

					FTL.AST.Placeable placeable = new FTL.AST.Placeable();
					// parse all placeable-expressions
					do {
						WhiteSpace.Parse(cs);
						// optional newline
						if(CharStream.IsNL(cs.PeekNext())) {
							NewLine.Parse(cs);
							WhiteSpace.Parse(cs);
						}
						placeable.AddExpression(PlaceableExpression.Parse(cs));
						WhiteSpace.Parse(cs);
						if(cs.PeekNext() != ',') {
							break;
						}

						// keep going, until we have no more commas
						cs.SkipNext();
					} while(true);

					// skip optional space
					WhiteSpace.Parse(cs);
					// optional newline
					if(CharStream.IsNL(cs.PeekNext())) {
						NewLine.Parse(cs);
						WhiteSpace.Parse(cs);
					}

					cs.SkipCharacter(POSTFIX);

					result = placeable;
					return true;
				}

				public static bool Peek(CharStream cs)
				{
					return cs.PeekNext() == PREFIX;
				}
				
				private const char PREFIX = '{';
				private const char POSTFIX = '}';
			}
		}
	}
}