// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// epresents the Root AST Element, containing all entries
			/// </summary>
			public sealed class Body : INode
			{	
				/// <summary>
				/// Returns the most optimized form of itself.
				/// </summary>
				public INode Optimize()
				{
					throw new NotImplementedException();
				}

				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(TextWriter writer)
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}