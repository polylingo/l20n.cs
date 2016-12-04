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
			/// The interface for all AST Object Types used in the AST of L20n.
			/// </summary>
			public interface INode
			{
				/// <summary>
				/// Returns the most optimized form of itself.
				/// </summary>
				INode Optimize();

				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				void Serialize(Writer writer);
			}
		}
	}
}