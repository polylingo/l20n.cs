// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;

using L20n.Exceptions;

namespace L20n
{
	namespace IO
	{
		/// <summary>
		/// Wraps around a TextReader, and is used to serialize a L20n (FTL) AST.
		/// </summary>
		public sealed class Writer
		{
			public Writer(TextWriter writer)
			{
				m_Writer = writer;
				m_Indention = 0;
			}
			
			public void Write(char c) {
				m_Writer.Write(c);
			}

			public void Write(string str) {
				m_Writer.Write(str);
			}
			
			public void Writef(string fmt, params Object[] argv) {
				m_Writer.Write(fmt, argv);
			}
			
			public void Writeln(string str) {
				m_Writer.Write(str + NEWLINE);
			}

			public void WriteIndention() {
				for(int i = 0; i < m_Indention; ++i)
					m_Writer.Write(INDENTION);
			}

			public void IncreaseIndention()
			{
				++m_Indention;
			}

			public void DecreaseIndention()
			{
				if(m_Indention <= 0) {
					throw new SerializeException(
						"Trying to decrease indention, while no indention was available");
				}

				--m_Indention;
			}

			public override string ToString()
			{
				return m_Writer.ToString();
			}

			private int m_Indention;
			private readonly TextWriter m_Writer;

			private const string INDENTION = "  ";
			private const string NEWLINE = "\n";
		}
	}
}