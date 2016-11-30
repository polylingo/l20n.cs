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
			/// The combinator parser used to parse a (select-)expression.
			/// </summary>
			public static class PlaceableExpression
			{
				public static FTL.AST.INode Parse(CharStream cs)
				{	
					FTL.AST.INode expression = Expresson.Parse(cs);
					bool wsSkipped = WhiteSpace.PeekAndSkip(cs);
					
					if(cs.PeekNext() != SEPERATOR[0]) {
						// it's not a select expression, so let's return early
						return expression;
					}
					
					// it must be a select expression
					
					// make sure we have at least 1 space
					if(wsSkipped) {
						throw cs.CreateException(String.Format(
							"require at least 1 space before '{0}' in a select-expression", SEPERATOR), null);
					}
					
					cs.SkipString(SEPERATOR);

					// we expect now a memberList (REQUIRED)
					FTL.AST.MemberList memberList = MemberList.Parse(cs);

					// return it all
					return new FTL.AST.SelectExpression(expression, memberList);
				}

				private const string SEPERATOR = "->";
			}
		}
	}
}