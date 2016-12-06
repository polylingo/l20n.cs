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
			/// The combinator parser used to parse an entry.
			/// An entry could be a comment or a section, but should usually be a message.
			/// </summary>
			public static class Entry
			{
				public static bool PeekAndParse(CharStream cs, Context ctx, out L20n.FTL.AST.INode result)
				{
					WhiteSpace.PeekAndSkip(cs);

					if (Message.PeekAndParse(cs, ctx, out result))
						return true;
					
					if (Section.PeekAndParse(cs, ctx, out result))
						return true;
					
					if (Comment.PeekAndParse(cs, ctx, out result))
						return true;
					
					result = null;
					return false;
				}
			}
		}
	}
}