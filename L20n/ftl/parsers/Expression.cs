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
			/// The combinator parser used to parse an expression.
			/// An expression can be a quoted-pattern, number, identifier,
			/// variable, call-expression and member-expression.
			/// </summary>
			public static class Expresson
			{
				public static FTL.AST.INode Parse(CharStream stream)
				{
					FTL.AST.INode result;
						
					if(Identifier.PeekAndParse(stream, out result)) {
						return ParseWithIdentifier(stream, result as FTL.AST.StringPrimitive);
					}
						
					if(Variable.PeekAndParse(stream, out result))
						return result;
						
					if(QuotedPattern.PeekAndParse(stream, out result))
						return result;
						
					if(Number.PeekAndParse(stream, out result))
						return result;
						
					throw stream.CreateException(
							"no <expression> could be found, while one was expected", null);
				}

				public static FTL.AST.INode ParseWithIdentifier(
					CharStream stream, FTL.AST.StringPrimitive identifier)
				{
					if(MemberExpression.Peek(stream))
						return MemberExpression.Parse(stream, identifier);
					
					if(CallExpression.Peek(stream))
						return CallExpression.Parse(stream, identifier);
					
					return identifier;
				}
			}
		}
	}
}