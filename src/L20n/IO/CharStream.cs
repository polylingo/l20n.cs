// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Collections.Generic;

using L20n.Exceptions;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;

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
			/// Returns the current buffer position in the stream.
			/// </summary>
			public int Position {
				get { return m_Stream.GetBufferPosition(); }
			}
			
			public static bool IsNL(char c)
			{
				return c == '\r' || c == '\n';
			}

			public static bool IsEOF(char c)
			{
				return c == '\0';
			}
						
			/// <summary>
			/// Creates a <see cref="L20n.IO.CharStream"/> instance based on a given string buffer.
			/// </summary>
			public CharStream(StreamReader stream, string path = null)
			{
				m_Stream = new BufferedStreamReader(stream);
				m_Buffer = new List<char>();
				m_BufferBlock = new char[8];
			}
			
			/// <summary>
			/// Peeks the next Character.
			/// </summary>
			public char PeekNext()
			{
				return m_Stream.PeekNext();
			}
			
			/// <summary>
			/// Reads the next Character.
			/// </summary>
			/// <remarks>
			/// \r\n counts as one and will be always returns as `NL`
			/// </remarks>
			public char ReadNext()
			{
				return m_Stream.ReadNext();
			}

			/// <summary>
			/// Reads the block of size n;
			/// </summary>
			public string ReadBlock(int n)
			{
				if(m_BufferBlock.Length < n)
					m_BufferBlock = new char[n];
				for(int i = 0; i < n; i++) {
					if(EndOfStream())
						break;
					m_BufferBlock[i] = ReadNext();
				}
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
			/// Reads an entire line.
			/// </summary>
			/// <remarks>
			/// The newline character of the line is skipped if it exists,
			/// but is not part of the returned string.
			/// </remarks>
			public string ReadLine()
			{
				string output = ReadUntil(IsNL);
				char next = PeekNext();
				if(next == '\n') {
					SkipNext();
				} else if(next == '\r') {
					SkipNext();
					if(PeekNext() == '\n')
						SkipNext();
				}

				return output;
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
			/// Reads the stream until the end.
			/// </summary>
			public string ReadUntilEnd()
			{
				return m_Stream.ReadUntilEnd();
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
				char next = PeekNext();
				if(next != expected) {
					string nextout = next.ToString();
					if(next == '\0')
						nextout = "EOF";

					string msg = string.Format(@"next character was `{0}`, while `{1}` was expected", nextout, expected);
					throw CreateException(msg, null);
				}
				SkipNext();
			}

			/// <summary>
			/// Skips as long as EOF is not reached and the predicate is satisfied
			/// </summary>
			public int SkipWhile(CharPredicate predicate)
			{
				int n = 0;
				while(!EndOfStream() && predicate(PeekNext())) {
					SkipNext();
					n++;
				}

				return n;
			}
			
			/// <summary>
			/// Skips as long as predicate is dissatisfied or EOF is reached
			/// </summary>
			public int SkipUntil(CharPredicate predicate)
			{
				int n = 0;

				while(!EndOfStream() && !predicate(PeekNext())) {
					SkipNext();
					n++;
				}

				return n;
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
			/// Rewinds the internal stream.
			/// Note that the given position is the buffer position.
			/// </summary>
			public void Rewind(int pos)
			{
				m_Stream.RewindBuffer(pos);
			}
						
			/// <summary>
			/// Returns <c>true</c> if the stream has no more characters left,
			/// <c>false</c> otherwise.
			/// </summary>
			public bool EndOfStream()
			{
				return m_Stream.EndOfStream();
			}
						
			/// <summary>
			/// Returns an exception with a given or default message,
			/// for either the current or offset position.
			/// </summary>
			public ParseException CreateException(string msg, Exception e)
			{
				int bufferPos = Position;
				string context = ReadBlock(20);
				if(!EndOfStream())
					context += "...";
				Rewind(bufferPos);

				return new ParseException(
					String.Format("Parse Exception near {0}: {1}",
				              ToLiteral(context.Replace("\0", "")), ToLiteral(msg)), e);
			}
						
			/// <summary>
			/// Clears buffer and disposes the current underlying stream.
			/// </summary>
			public void Dispose()
			{
				m_Buffer.Clear();
			}

			private static string ToLiteral(string input)
			{
				using(var writer = new StringWriter()) {
					using(var provider = CodeDomProvider.CreateProvider("CSharp")) {
						provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
						return writer.ToString();
					}
				}
			}
						
			// used to allow the user of this class to define its own predicate given a char.
			public delegate bool CharPredicate(char c);
						
			// the buffer object containing all the chars to be "streamed"
			private BufferedStreamReader m_Stream = null;
			// a buffer used when reading an unknown amount of characters
			private List<char> m_Buffer;
			private char[] m_BufferBlock;
		}
	}
}