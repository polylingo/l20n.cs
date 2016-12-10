// // Glen De Cauwsemaecker licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.
using System;
using System.IO;

using L20n.Exceptions;

namespace L20n
{
	namespace IO
	{
		/// <summary>
		/// The BufferedStreamReader allows you to read from a StreamReader,
		/// with the advantage of having a small buffer zone that buffers everything that gets read,
		/// such that rewinding to a previous buffered point is possible.
		/// </summary>
		public sealed class BufferedStreamReader
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20n.IO.BufferedStreamReader"/> class.
			/// </summary>
			public BufferedStreamReader(StreamReader reader)
			{
				m_StreamReader = reader;
				m_Pos = 0;
				m_BufferSize = 0;
				m_BufferLimit = NORMAL_BUFFER_LIMIT;
				m_CharBuffer = new char[m_BufferLimit];
			}

			/// <summary>
			/// Reads the next buffered or unbuffered character.
			/// Throws an exception in case no character is available (EOF).
			/// </summary>
			public char ReadNext()
			{
				char output;
				if(m_Pos != m_BufferSize) {
					// return a buffered char
					return m_CharBuffer[m_Pos++];
				}

				// most of the times however,
				// we'll want to read fresh content

				int i = m_StreamReader.Read();
				if(i == -1)
					throw new ParseException("unexpected EOF");

				output = (char)i;

				// if our buffer is full
				if(m_Pos == m_BufferLimit) {
					m_BufferLimit *= 2;
					Array.Resize<char>(ref m_CharBuffer, m_BufferLimit);
				}

				// buffer the read character
				m_CharBuffer[m_Pos++] = output;
				m_BufferSize = m_Pos;

				return output;
			}

			/// <summary>
			/// Reads all buffered (starting from the current buffer position)
			/// and non-buffered content.
			/// Note that nothing will be left to read or peek, after this method returns its result.
			/// </summary>
			public string ReadUntilEnd()
			{
				string output = "";
				if(m_Pos != m_BufferSize)
					output += new string(m_CharBuffer, m_Pos, m_BufferSize - m_Pos);

				m_Pos = 0;
				m_BufferSize = 0;

				if(!m_StreamReader.EndOfStream)
					output += m_StreamReader.ReadToEnd();
				return output;
			}

			/// <summary>
			/// Peeks the next available character.
			/// </summary>
			public char PeekNext()
			{
				if(m_Pos == m_BufferSize) {
					int i = m_StreamReader.Peek();
					return i == -1 ? '\0' : (char)i;
				}

				return m_CharBuffer[m_Pos];
			}

			/// <summary>
			/// Rewinds the buffer to the given position,
			/// an exception gets thrown in case the given position is not within the legal bounds.
			/// </summary>
			public void RewindBuffer(int pos)
			{
				if(pos < 0 || pos >= m_CharBuffer.Length)
					throw new Exception(String.Format("{0} is an invalid bufffer position"));

				m_Pos = pos;
			}

			/// <summary>
			/// Returns the current buffer position.
			/// Note that this is not the actual position in text, and should not be relied upon
			/// for information given to the outside.
			/// </summary>
			public int GetBufferPosition()
			{
				return m_Pos;
			}

			public bool EndOfStream()
			{
				return m_Pos == m_BufferSize && m_StreamReader.EndOfStream;
			}

			// the internal buffer pos (NOT THE GLBOAL POS)
			private int m_Pos;
			// the current upper limit, equal to the pos of the last registered char
			private int m_BufferSize;
			// the size of the buffer
			private int m_BufferLimit;
			// the char buffer
			private char[] m_CharBuffer;

			// the default size of the buffer
			// the buffer will be normalized to this size after each flush
			// in order to prevent abnormalities from unfluincing its after life
			const int NORMAL_BUFFER_LIMIT = 512;

			// the actual internal stream reader that gets read from
			// in case the buffer position reached its limits (the normal situation)
			private readonly StreamReader m_StreamReader;
		}
	}
}
