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
			/// The AST representation for an /arglist/.
			/// More Information: <see cref="L20n.FTL.Parsers.CallExpression"/>
			/// </summary>
			public sealed class ArgumentsList : INode
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.ArgumentsList"/> class.
				/// </summary>
				public ArgumentsList()
				{
					m_Arguments = new List<INode>();
				}

				/// <summary>
				/// Adds the argument to the list of already existing arguments.
				/// </summary>
				public void AddArgument(INode argument)
				{
					m_Arguments.Add(argument);
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					// if no arguments, early exit!
					if(m_Arguments.Count == 0)
						return;

					int lastPos = m_Arguments.Count - 1;
					// write all arguments except last one
					int bound = lastPos;
					for(int i = 0; i < bound; ++i) {
						m_Arguments[i].Serialize(writer);
						writer.Write(", ");
					}

					// write last argument
					m_Arguments[lastPos].Serialize(writer);
				}
				
				private readonly List<INode> m_Arguments;
			}
		}
	}
}