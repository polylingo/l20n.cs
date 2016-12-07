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
			/// The combinator parser used to parse a member-key.
			/// 
			///  ((identifier '/')? keyword) | number
			/// </summary>
			public static class Memberkey
			{
				public static FTL.AST.INode Parse(CharStream cs)
				{
					FTL.AST.INode node;

					// could be <identifier>/<keyword>
					if(Identifier.PeekAndParse(cs, out node)) {
						// do we also require a keyword?
						if(cs.PeekNext() == '/') {
							cs.SkipNext(); // '/'
							FTL.AST.StringPrimitive keyword = Keyword.Parse(cs);
							return new FTL.AST.Attribute(
								node as FTL.AST.StringPrimitive, // node == identifier
								keyword);
						}

						// it's just an identifier that's used as a member-key
						return node;
					}

					// it must a number that's used as a member-key
					node = Number.Parse(cs);
					return node;
				}
			}
		}
	}
}