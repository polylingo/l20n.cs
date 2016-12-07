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
			/// 
			/// expression | select-expression
			/// select-expression ::= expression __ ' ->' __ member-list
			/// </summary>
			public static class PlaceableExpression
			{
				public static FTL.AST.INode Parse(CharStream cs)
				{	
					FTL.AST.INode expression = Expresson.Parse(cs);

					cs.StartBuffering();
					WhiteSpace.Parse(cs);
					cs.StopBuffering();
					
					if(cs.PeekNextUnbuffered() != SEPERATOR[0]) {
						// it's not a select expression, so let's return early
						return expression;
					}

					cs.FlushBuffer();
					
					// it must be a select expression
					
					cs.SkipString(SEPERATOR);

					WhiteSpace.Parse(cs);

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