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
			/// The AST representation for <quoted-text>.
			/// </summary>
			public sealed class QuotedText : INode
			{
				/// <summary>
				/// Gets the internal string as a constant.
				/// </summary>
				public string Value
				{
					get { return m_Value; }
				}
				
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.AST.QuotedText"/> class.
				/// </summary>
				public QuotedText(string value)
				{
					m_Value = value;
				}
				
				/// <summary>
				/// Writes its content and the content of its children.
				/// </summary>
				public void Serialize(Writer writer)
				{
					writer.Writef("\"{0}\"", m_Value);
				}
				
				private readonly string m_Value;
			}
		}
	}
}