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
			/// The combinator parser used to parse a section.
			/// </summary>
			public static class Section
			{
					
				public static bool PeekAndParse(CharStream stream, Context ctx, out FTL.AST.INode section)
				{
					if(!(stream.PeekNext() == PREFIX[0])) {
						section = null;
						return false;
					}
						
					if(ctx.ASTType == Context.ASTTypes.Full) {
						section = Parse(stream);
						return true;
					}
						
					Skip(stream);
					section = null;
					return true;
				}

				private static FTL.AST.Section Parse(CharStream cs)
				{
					WhiteSpace.PeekAndSkip(cs);
					cs.SkipString(PREFIX);
					WhiteSpace.PeekAndSkip(cs);
					
					var keyword = Keyword.Parse(cs);
					
					WhiteSpace.PeekAndSkip(cs);
					cs.SkipString(POSTFIX);
					WhiteSpace.PeekAndSkip(cs);
					NewLine.Skip(cs);
					
					return new FTL.AST.Section(keyword);
				}
				
				private static void Skip(CharStream cs)
				{
					WhiteSpace.PeekAndSkip(cs);
					cs.SkipString(PREFIX);
					// this does mean that for applications the section could be bullox, but that's fine
					// tooling should prevent such things
					cs.SkipWhile(IsNotNewLine);
					NewLine.Skip(cs);
				}

				private const string PREFIX = "[[";
				private const string POSTFIX = "]]";
					
				private static bool IsNotNewLine(char c)
				{
					return CharStream.IsNL(c);
				}
			}
		}
	}
}