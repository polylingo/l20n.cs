// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{
			/// <summary>
			/// The default FTL Parser
			/// 
			/// body ::= (entry NL)* (entry)? EOF;
			/// </summary>
			public sealed class Parser : IParser
			{	
				/// <summary>
				/// Parses an entire charstream into an FTL AST
				/// </summary>
				public FTL.AST.Body Parse(StreamReader reader, Context ctx)
				{
					CharStream cs = new CharStream(reader);

					FTL.AST.Body root = new FTL.AST.Body(ctx);
					L20n.FTL.AST.INode entry;
					
					while(Entry.PeekAndParse(cs, ctx, out entry)) {
						if(entry != null) // could have been ommitted because of partial AST
							root.AddEntry(entry);

						if(!CharStream.IsNL(cs.PeekNext()))
							break;

						NewLine.Parse(cs);
					}

					if(!cs.EndOfStream())
						throw cs.CreateException(
							"didn't reach end of stream while that was expected: `" + cs.ReadUntilEnd() + "`",
							null);

					return root;
				}

				public string ParserName()
				{
					return "Default FTL Parser";
				}
			}
		}
	}
}