// // Glen De Cauwsemaecker licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.
using System;

namespace L20n
{
	namespace FTL
	{
		namespace Parsers
		{
			/// <summary>
			/// the Context is used by some of the L20n FTL Parsers
			/// and contains parser configuration and temporary state
			/// </summary>
			public sealed class Context
			{	
				public enum ASTTypes : byte
				{
					Full = 1, // used by Tools that require the entire AST
					Partial = 2, // used by Applications that require only the messages
				}
				
				/// <summary>
				/// Gets the type of the AST.
				/// </summary>
				public ASTTypes ASTType
				{
					get { return m_ASTType; }
				}
				
				/// <summary>
				/// Initializes a new instance of the <see cref="L20n.FTL.Parsers.Context"/> class.
				/// </summary>
				public Context(ASTTypes type)
				{
					m_ASTType = type;
				}
				
				private readonly ASTTypes m_ASTType;
			}
		}
	}
}
