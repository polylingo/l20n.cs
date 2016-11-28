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
			/// The default FTL Parser
			/// </summary>
			public sealed class Parser : IParser
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.Parsers.Parser"/> class.
				/// </summary>
				public Parser()
				{
				}

				/// <summary>
				/// Parses an entire charstream into an FTL AST
				/// </summary>
				public FTL.AST.Body Parse(CharStream cs, Context ctx)
				{
					FTL.AST.Body root = new FTL.AST.Body();
					return root;
				}

				public void Dispose()
				{
				}
			}
		}
	}
}