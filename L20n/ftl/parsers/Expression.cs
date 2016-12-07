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
			/// 
			/// identifier | member-expression | call-expression | variable | number | quoted-pattern
			/// </summary>
			public static class Expresson
			{
				public static FTL.AST.INode Parse(CharStream stream)
				{
					FTL.AST.INode result;
					if(Identifier.PeekAndParse(stream, out result))
						return ParseWithIdentifier(stream, result as FTL.AST.StringPrimitive);
						
					return ParseNoneIdentifier(stream);
				}

				public static FTL.AST.INode ParseNoneIdentifier(CharStream stream)
				{
					FTL.AST.INode result;

					if(Variable.PeekAndParse(stream, out result))
						return result;
					
					if(Number.PeekAndParse(stream, out result))
						return result;
					
					return Pattern.ParseQuoted(stream);
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