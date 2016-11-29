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
			/// The combinator parser used to parse an entity (similar to L10n's entity).
			/// </summary>
			public static class Message
			{
				public static bool PeekAndParse(CharStream cs, Context ctx, out L20n.FTL.AST.INode comment)
				{
					if(Identifier.PeekAndParse(cs, out comment)) {
						comment = Parse(cs, ctx, comment as FTL.AST.StringPrimitive);
						return true;
					}

					return false;
				}

				private static FTL.AST.Entity Parse(CharStream cs, Context ctx, FTL.AST.StringPrimitive identifier)
				{
					WhiteSpace.PeekAndSkip(cs);
					cs.SkipCharacter('=');
					WhiteSpace.PeekAndSkip(cs);

					throw new NotImplementedException();
				}
			}
		}
	}
}