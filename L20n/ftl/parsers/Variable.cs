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
			/// The combinator parser used to parse a variable.
			/// A variable contains an identifier.
			/// </summary>
			public static class Variable
			{
				public static L20n.FTL.AST.Variable Parse(CharStream cs)
				{
					cs.SkipCharacter(PREFIX);
					L20n.FTL.AST.StringPrimitive identifier = Identifier.Parse(cs);
					return new L20n.FTL.AST.Variable(identifier);
				}
					
				public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode result)
				{
					if(stream.PeekNext() != PREFIX) {
						result = null;
						return false;
					}
						
					result = Parse(stream);
					return true;
				}

				private const char PREFIX = '$';
			}
		}
	}
}