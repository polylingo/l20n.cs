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
			/// 
			/// identifier __ '=' __ (pattern | pattern member-list | member-list)
			/// </summary>
			public static class Message
			{
				public static bool PeekAndParse(CharStream cs, Context ctx, out L20n.FTL.AST.INode entity)
				{
					if(Identifier.PeekAndParse(cs, out entity)) {
						entity = Parse(cs, ctx, entity as FTL.AST.StringPrimitive);
						return true;
					}

					return false;
				}

				private static FTL.AST.Entity Parse(CharStream cs, Context ctx, FTL.AST.StringPrimitive identifier)
				{
					WhiteSpace.Parse(cs);
					cs.SkipCharacter('=');
					WhiteSpace.Parse(cs);

					FTL.AST.Pattern pattern = null;
					// check if we have a Pattern available
					bool hasPattern = Pattern.PeekAndParse(cs, out pattern);

					FTL.AST.MemberList memberList;
					bool parsedMemberList = MemberList.PeekAndParse(cs, out memberList);
					if(!parsedMemberList && !hasPattern) {
						throw cs.CreateException(
							"member-list was expected, as no pattern was found", null);
					}

					return new FTL.AST.Entity(identifier, pattern, memberList);
				}
			}
		}
	}
}