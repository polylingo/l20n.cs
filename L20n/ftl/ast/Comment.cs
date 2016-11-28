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
			/// The AST representation for a Comment.
			/// More Information: <see cref="L20n.FTL.Parsers.Comment"/>
			/// </summary>
			public sealed class Comment : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Comment"/> class.
				/// </summary>
				/// <param name="value">Value.</param>
				public Comment(string value)
				{
					m_Value = value;
				}

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
					writer.WriteLine("#" + m_Value);
				}
				
				private readonly string m_Value;
			}
		}
	}
}