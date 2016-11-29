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
			/// The combinator parser used to parse a call-expression.
			/// </summary>
			public static class CallExpression
			{
				public static FTL.AST.CallExpression Parse(CharStream cs, FTL.AST.StringPrimitive builtin)
				{
					if(!Builtin.IsValid(builtin.Value)) {
						throw cs.CreateException(
							string.Format("{0} is not a valid builtin, while one was expected", builtin.Value),
							null,
							builtin.Value.Length * -1);
					}

					cs.SkipCharacter(PREFIX);
					FTL.AST.ArgumentsList arguments = new FTL.AST.ArgumentsList();
					
					while(cs.PeekNext() != POSTFIX) {
						WhiteSpace.PeekAndSkip(cs);
						arguments.AddArgument(Argument.Parse(cs));
						WhiteSpace.PeekAndSkip(cs);
						if(cs.PeekNext() != ',')
							break; // exit early, as no more arguments are expected
						cs.SkipNext(); // skip ','
					}
					
					// make sure last non-ws char is ')',
					// otherwise something went wrong
					cs.SkipCharacter(POSTFIX);
					
					return new FTL.AST.CallExpression(builtin, arguments);
				}
				
				public static bool Peek(CharStream cs)
				{
					return cs.PeekNext() == PREFIX;
				}
				
				private const char PREFIX = '(';
				private const char POSTFIX = ')';
			}
		}
	}
}