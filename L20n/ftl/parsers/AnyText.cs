// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{
			/// <summary>
			/// Parser used to parse all types of text
			/// </summary>
			public static class AnyText
			{
				public static FTL.AST.StringPrimitive ParseUnquoted(CharStream cs)
				{
					s_Buffer.Clear();
					WhiteSpace.PeekAndSkip(cs);

					bool allowCB = false;
					char next = cs.PeekNext();
					while(!CharStream.IsNL(next) && next != CharStream.EOF &&
					      (next != '{' || allowCB)) {
						s_Buffer.Add(next);
						cs.SkipNext();
						allowCB = !allowCB && next == '\\';
						next = cs.PeekNext();
					}

					if(s_Buffer.Count == 0) {
						throw cs.CreateException(
							"no unquoted text could be parsed, while this was expected", null);
					}

					return new FTL.AST.StringPrimitive(new string(s_Buffer.ToArray()));
				}

				public static FTL.AST.QuotedText ParseQuoted(CharStream cs)
				{
					s_Buffer.Clear();
					WhiteSpace.PeekAndSkip(cs);
					
					bool allowSC = false;
					char next = cs.PeekNext();
					while(!CharStream.IsNL(next) && next != CharStream.EOF &&
					      (allowSC || (next != '{' && next != '"'))) {
						s_Buffer.Add(next);
						cs.SkipNext();
						allowSC = !allowSC && next == '\\';
						next = cs.PeekNext();
					}
					
					if(s_Buffer.Count == 0) {
						throw cs.CreateException(
							"no quoted text could be parsed, while this was expected", null);
					}
					
					return new FTL.AST.QuotedText(new string(s_Buffer.ToArray()));
				}

				public static bool PeekAndParseBlock(CharStream cs, out FTL.AST.INode result)
				{
					WhiteSpace.PeekAndSkip(cs);
					if(!CharStream.IsNL(cs.PeekNext())) {
						result = null;
						return false;
					}

					FTL.AST.BlockText blockText = new FTL.AST.BlockText();
					NewLine.Skip(cs);
					WhiteSpace.PeekAndSkip(cs);
					do {
						cs.SkipCharacter('|');
						WhiteSpace.PeekAndSkip(cs);
						blockText.AddLine(Pattern.ParseUnquoted(cs));
						WhiteSpace.PeekAndSkip(cs);
						NewLine.Skip(cs);
						WhiteSpace.PeekAndSkip(cs);
					} while(cs.PeekNext() == '|');

					result = blockText;
					return true;
				}

				private static List<char> s_Buffer = new List<char>(80);
			}
		}
	}
}