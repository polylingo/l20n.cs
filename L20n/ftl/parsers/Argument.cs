// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.IO;
using L20n.Exceptions;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{	
			/// <summary>
			/// The combinator parser used to parse an Argument.
			/// An argument can be either an expression or a keyword-argument.
			/// </summary>
			public static class Argument
			{
				public static FTL.AST.INode Parse(CharStream cs)
				{
					FTL.AST.INode result;
						
					// if it's an identifier, it could be either simply be an identifier,
					// or it could actually be a keyword-argument
					if(Identifier.PeekAndParse(cs, out result)) {
						// ignore any whitespace
						WhiteSpace.PeekAndSkip(cs);
							
						// if we now encounter a `=` char, we'll assume it's a keyword-argument,
						// and finish the parsing of that element,
						// otherwise we'll assume it's simply an identifier and return early
						if(cs.PeekNext() != '=')
							return Expresson.ParseWithIdentifier(cs, result as FTL.AST.StringPrimitive);

						cs.SkipNext();
						WhiteSpace.PeekAndSkip(cs);
							
						FTL.AST.Pattern pattern = Pattern.ParseQuoted(cs);
						return new FTL.AST.KeywordArgument(
								result as L20n.FTL.AST.StringPrimitive,
								pattern);
					}
						
					// it's not an identifier, so is must be any non-identifier expression
					return Expresson.Parse(cs);
				}
			}
		}
	}
}