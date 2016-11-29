// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;

using L20n.Exceptions;

namespace L20n
{
	namespace FTL
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a call-expression.
			/// More Information: <see cref="L20n.FTL.Parsers.CallExpression"/>
			/// </summary>
			public sealed class CallExpression : INode
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.CallExpression"/> class.
				/// </summary>
				public CallExpression(StringPrimitive builtin, ArgumentsList arguments)
				{
					m_Builtin = builtin;
					m_Arguments = arguments;
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
					m_Builtin.Serialize(writer);
					writer.Write('(');
					m_Arguments.Serialize(writer);
					writer.Write(')');
				}
				
				private readonly StringPrimitive m_Builtin;
				private readonly ArgumentsList m_Arguments;
			}
		}
	}
}