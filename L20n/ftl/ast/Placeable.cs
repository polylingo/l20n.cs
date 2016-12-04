// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20n.IO;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a placeable.
			/// </summary>
			public sealed class Placeable : INode
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.Placeable"/> class.
				/// </summary>
				public Placeable()
				{
					m_Expressions = new List<INode>();
				}
				
				/// <summary>
				/// Adds the expression to the list of already existing expressions.
				/// </summary>
				public void AddExpression(INode expression)
				{
					m_Expressions.Add(expression);
				}
				
				/// <summary>
				/// TODO
				/// </summary>
				public INode Optimize()
				{
					throw new NotImplementedException();
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{	
					writer.Write("{ ");
					int lastPos = m_Expressions.Count - 1;
					// write all expressions except last one
					int bound = lastPos;
					for(int i = 0; i < bound; ++i) {
						m_Expressions[i].Serialize(writer);
						writer.Write(", ");
					}
					
					// write last expression
					m_Expressions[lastPos].Serialize(writer);

					writer.Write(" }");
				}
				
				private readonly List<INode> m_Expressions;
			}
		}
	}
}