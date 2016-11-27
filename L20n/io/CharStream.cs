// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Collections.Generic;

using L20n.Exceptions;
using System.Text;

namespace L20n
{
	namespace IO
	{
		/// <summary>
		/// <see cref="L20n.IO.CharStream"/> is a utility class to make the
		/// parsing logic easier and keep the streams-specific logic centralized
		/// and seperated from the specialized parser logic.
		/// </summary>
		/// <remarks>
		/// This class is not thread-safe, and should not be used in concurrent environments.
		/// </remarks>
		public class CharStream : IDisposable
		{
			/// <summary>
			/// Returns the path to the actual resources this instance is streaming.
			/// </summary>
			public string Path {
				get { return m_Path; }
			}
						
			/// <summary>
			/// Returns the current position in the stream.
			/// </summary>
			public int Position {
				get { return m_Position; }
			}

			public static readonly char NOP = '\0';
			public static readonly char NL = '\n';
			
			public static bool IsNL(char c)
			{
				return c == '\r' || c == '\n';
			}
						
			/// <summary>
			/// Creates a <see cref="L20n.IO.CharStream"/> instance based on a given string buffer.
			/// </summary>
			public CharStream(StreamReader stream, string path = null)
			{
				m_Path = path;
				m_Stream = stream;
				m_Position = 0;
				m_NewLineCount = 0;
				m_NewLineStartPosition = 0;
				m_Buffer = new List<char>();
				m_BufferBlock = new char[8];
			}
						
			/// <summary>
			/// Creates a <see cref="L20n.IO.CharStream"/> instance with the buffered content
			/// read from the resource found at the given path.
			/// </summary>
			public CharStream(String path)
			{
				m_Path = path;
				m_Stream = StreamReaderFactory.Create(path);
				m_Position = 0;
				m_NewLineCount = 0;
				m_NewLineStartPosition = 0;
				m_Buffer = new List<char>();
				m_BufferBlock = new char[8];
			}
			
			/// <summary>
			/// Peeks the next Character.
			/// </summary>
			public char PeekNext()
			{
				try {
					return (char)m_Stream.Peek();
				} catch(Exception e) {
					throw CreateException("next character could not be peeked", e);
				}
			}
			
			/// <summary>
			/// Reads the next Character.
			/// </summary>
			/// <remarks>
			/// \r\n counts as one and will be always returns as `NL`
			/// </remarks>
			public char ReadNext()
			{
				try {
					++m_Position;
					char next = (char)m_Stream.Read();
					if(next == '\r') {
						if(PeekNext() == NL) {
							next = (char)m_Stream.Read(); // we count '\r\n' as 1 char
						}
					}

					if(IsNL(next)) {
						m_NewLineCount++;
						m_NewLineStartPosition = m_Position;
					}

					return next;
				} catch(Exception e) {
					throw CreateException("next character could not be read", e);
				}
			}

			/// <summary>
			/// Reads the block of size n;
			/// </summary>
			public string ReadBlock(int n)
			{
				if(m_BufferBlock.Length < n)
					m_BufferBlock = new char[n];
				for(int i = 0; i < n; i++)
					m_BufferBlock[i] = ReadNext();
				return new string(m_BufferBlock, 0, n);
			}
			
			/// <summary>
			/// Reads until EOF is reached or
			/// until predicate does not get satisfied.
			/// </summary>
			public string ReadWhile(CharPredicate predicate)
			{
				m_Buffer.Clear();
				while(!EndOfStream() && predicate(PeekNext()))
					m_Buffer.Add(ReadNext());
				return new string(m_Buffer.ToArray());
			}
			
			/// <summary>
			/// Reads until EOF is reached or
			/// until predicate gets satisfied.
			/// </summary>
			public string ReadUntil(CharPredicate predicate)
			{
				m_Buffer.Clear();
				while(!EndOfStream() && !predicate(PeekNext()))
					m_Buffer.Add(ReadNext());
				return new string(m_Buffer.ToArray());
			}

			/// <summary>
			/// Reads an entire line.
			/// </summary>
			public string ReadLine()
			{
				string output = m_Stream.ReadLine();
				if(output != null) {
					m_Position += output.Length + 1;
					++m_NewLineCount;
					m_NewLineStartPosition = m_Position;
				}

				return output;
			}

			public string ReadUntilEnd()
			{
				try {
					string s = m_Stream.ReadToEnd();
					if(s != null) {
						m_Position += s.Length;

						for(int i = 0; i < s.Length; ++i) {
							if(s[i] == '\n') {
								++m_NewLineCount;
								m_NewLineStartPosition = m_Position;
								continue;
							}

							if(s[i] == '\r') {
								if(i < s.Length - 1 && s[i + 1] == '\n') {
									++i;
								}
								
								++m_NewLineCount;
								m_NewLineStartPosition = m_Position;
							}
						}
					}
					
					return s;
				} catch(Exception e) {
					throw CreateException("could not read until end", e);
				}
			}

			/// <summary>
			/// Skips the next character.
			/// </summary>
			public void SkipNext()
			{
				ReadNext();
			}
			
			/// <summary>
			/// Skips the next N characters.
			/// </summary>
			public void SkipBlock(int n)
			{
				for(int i = 0; i < n; i++)
					ReadNext();
			}

			/// <summary>
			/// Skips the next expected character.
			/// </summary>
			public void SkipCharacter(char expected)
			{
				char next = ReadNext();
				if(next != expected)
					throw CreateException(
						string.Format("next character was {0}, while {1} was expected", next, expected), null, -1);
			}

			/// <summary>
			/// Skips as long as EOF is not reached and the predicate is satisfied
			/// </summary>
			public void SkipWhile(CharPredicate predicate)
			{
				while(!EndOfStream() && predicate(PeekNext()))
					SkipNext();
			}
			
			/// <summary>
			/// Skips as long as predicate is dissatisfied or EOF is reached
			/// </summary>
			public void SkipUntil(CharPredicate predicate)
			{
				while(!EndOfStream() && !predicate(PeekNext()))
					SkipNext();
			}

			/// <summary>
			/// Skips the entire expected string
			/// </summary>
			public void SkipString(string expected)
			{
				for(int i = 0; i < expected.Length; ++i)
					SkipCharacter(expected[i]);
			}
						
			/// <summary>
			/// Returns <c>true</c> if the stream has no more characters left,
			/// <c>false</c> otherwise.
			/// </summary>
			public bool EndOfStream()
			{
				return m_Stream.EndOfStream;
			}
						
			/// <summary>
			/// Computes a user-friendly position that gives both the Line and Column number,
			/// based on the given linear stream position.
			/// Result gets returned in a formatted string.
			/// </summary>
			public string ComputeDetailedPosition(int offset = 0)
			{
				int lineNumber = m_NewLineCount;
				int linePosition = (m_Position + offset) - m_NewLineStartPosition;
				return String.Format("L{0}:{1}", lineNumber + 1, linePosition + 1); // 1-based
			}
						
			/// <summary>
			/// Returns an exception with a given or default message,
			/// for either the current or offset position.
			/// </summary>
			public ParseException CreateException(string msg, Exception e, int offset = 0)
			{
				return new ParseException(
					String.Format("'{0}' at {1} is unexpected: {2}",
					(char)m_Stream.Peek(), ComputeDetailedPosition(offset), msg), e);
			}
						
			/// <summary>
			/// Clears buffer and disposes the current underlying stream.
			/// </summary>
			public void Dispose()
			{
				m_Buffer.Clear();
			}
						
			// used to allow the user of this class to define its own predicate given a char.
			public delegate bool CharPredicate(char c);
						
			// the path to the resource to be streamed
			private readonly string m_Path = null;
			// the buffer object containing all the chars to be "streamed"
			private StreamReader m_Stream = null;
			// the current position in the buffer
			private int m_Position;
			// the start position on the current line
			private int m_NewLineStartPosition;
			// the current amount of newlines
			private int m_NewLineCount;
			// a buffer used when reading an unknown amount of characters
			private List<char> m_Buffer;
			private char[] m_BufferBlock;
		}
	}
}