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
			/// represents the AST element for an entity, AKA <message>
			/// </summary>
			public sealed class Entity : INode
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