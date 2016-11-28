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
			/// The combinator parser used to parse a comment.
			/// </summary>
			public static class Comment
			{
				public static bool PeekAndParse(CharStream cs, Context ctx, out FTL.AST.INode comment)
				{
					if (cs.PeekNext() != '#')
					{
						comment = null;
						return false;
					}
					
					if (ctx.ASTType == Context.ASTTypes.Full)
					{
						comment = Parse(cs);
						return true;
					}
					
					ParseAndForget(cs);
					comment = null;
					return true;
				}
				
				private static L20n.FTL.AST.Comment Parse(CharStream cs)
				{
					cs.SkipCharacter('#');
					string value = cs.ReadWhile(IsNotNL);
					return new L20n.FTL.AST.Comment(value);
				}
				
				private static void ParseAndForget(CharStream stream)
				{
					stream.SkipCharacter('#');
					stream.SkipWhile(IsNotNL);
				}

				private static bool IsNotNL(char c)
				{
					return !CharStream.IsNL(c);
				}
			}
		}
	}
}