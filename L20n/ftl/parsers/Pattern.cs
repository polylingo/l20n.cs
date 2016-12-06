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
			/// A pattern can be quoted or unquoted.
			/// </summary>
			public static class Pattern
			{
				public static FTL.AST.Pattern Parse(CharStream cs)
				{
					if(cs.PeekNext() == '"')
						return ParseQuoted(cs);

					return ParseUnquoted(cs);
				}

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

					// [TODO] Fix Block
					//if(AnyText.PeekAndParseBlock(cs, out child))
					//	return child;

					// as long as we don't have a newline char,
					// we'll assume it's an unquoted-child
					char next = cs.PeekNext();
					if(next != CharStream.EOF && !CharStream.IsNL(next))
						return AnyText.ParseUnquoted(cs);

					// return null if no child could be found
					return null;
				}
			}
		}
	}
}