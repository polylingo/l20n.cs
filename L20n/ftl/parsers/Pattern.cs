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
			/// The combinator parser used to parse a pattern.
			/// 
			/// quoted-pattern | unquoted-pattern
			/// </summary>
			public static class Pattern
			{
				public static FTL.AST.Pattern Parse(CharStream cs)
				{
					if(cs.PeekNext() == '"')
						return ParseQuoted(cs);

					return ParseUnquoted(cs);
				}

				public static bool PeekAndParse(CharStream cs, out FTL.AST.Pattern pattern)
				{
					// A quoted-pattern is the easiest to detect, so we'll try that first
					if(cs.PeekNext() == '"') {
						pattern = ParseQuoted(cs);
						return true;
					}

					// it might still be an unquoted-pattern, but this is trickier to detect
					// first let's try to detect a placeable and block-text
					if(Placeable.Peek(cs) || AnyText.PeekBlockText(cs)) {
						pattern = ParseUnquoted(cs);
						return true;
					}

					// if not any of the above, the only thing left is unquoted-text
					int bufferPos = cs.Position;
					WhiteSpace.Parse(cs);
					char next = cs.PeekNext();
					cs.Rewind(bufferPos);
					if(!CharStream.IsEOF(next) && !CharStream.IsNL(next)) {
						pattern = ParseUnquoted(cs);
						return true;
					}

					pattern = null;
					return false;
				}

				// '"' (placeable | quoted-text)* '"'
				public static FTL.AST.Pattern ParseQuoted(CharStream cs)
				{
					cs.SkipCharacter('"');
					FTL.AST.Pattern pattern = new FTL.AST.Pattern(true);
					FTL.AST.INode child;
					
					while(cs.PeekNext() != '"') {
						if(Placeable.PeekAndParse(cs, out child)) {
							pattern.AddChild(child);
							continue;
						}

						// it's not a placeable, and we haven't seen a quote,
						// so it must be a quoted-text
						child = AnyText.ParseQuoted(cs);
						pattern.AddChild(child);
					}

					cs.SkipCharacter('"');
					return pattern;
				}

				// (unquoted-text | placeable | block-text)+
				public static FTL.AST.Pattern ParseUnquoted(CharStream cs)
				{
					FTL.AST.Pattern pattern = new FTL.AST.Pattern(false);
					FTL.AST.INode child = ParseUnquotedChild(cs);
					if(child == null) {
						throw cs.CreateException(
							"no unquoted child could be found, while at least one was expected", null);
					}

					do {
						pattern.AddChild(child);
						child = ParseUnquotedChild(cs);
					} while(child != null);

					return pattern;
				}
				
				private static FTL.AST.INode ParseUnquotedChild(CharStream cs)
				{
					FTL.AST.INode child;

					if(Placeable.PeekAndParse(cs, out child))
						return child;

					if(AnyText.PeekAndParseBlock(cs, out child))
						return child;

					// as long as we don't have a newline char,
					// we'll assume it's an unquoted-child
					if(AnyText.PeekUnquoted(cs))
						return AnyText.ParseUnquoted(cs);

					// return null if no child could be found
					return null;
				}
			}
		}
	}
}