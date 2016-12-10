// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a reference.
			/// </summary>
			public sealed class Reference : INode
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Reference"/> class.
				/// </summary>
				public Reference(StringPrimitive identifier)
				{
					m_Identifier = identifier;
				}
				
				/// <summary>
				/// Returns itself, as it can't be optimized any further.
				/// </summary>
				public INode Optimize()
				{
					return this;
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					m_Identifier.Serialize(writer);
				}
				
				private readonly StringPrimitive m_Identifier;
			}
		}
	}
}