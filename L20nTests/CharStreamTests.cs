// // Glen De Cauwsemaecker licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

using L20n.IO;
using L20n.Exceptions;

namespace L20nTests
{
	[TestFixture()]
	public sealed class CharStreamTests
	{
		[Test()]
		public void CSharpUncodeTests()
		{
			Assert.AreEqual("Hello, World!", NCS("Hello, World!").ReadLine());
			Assert.AreEqual("你好，世界！", NCS("你好，世界！").ReadLine());
			Assert.AreEqual("Chào thế giới!", NCS("Chào thế giới!").ReadLine());
			Assert.AreEqual("😍", NCS("😍").ReadLine());
		}

		[Test()]
		public void EmptyFile()
		{
			var streamReader = StreamReaderFactory.Create("../../resources/io/Empty.txt");
			var cs = new CharStream(streamReader);
			Assert.IsTrue(cs.EndOfStream());
		}

		[Test()]
		public void CharStreamFile()
		{
			var streamReader = StreamReaderFactory.Create("../../resources/io/CharStream.txt");
			var cs = new CharStream(streamReader);

			Assert.AreEqual('H', cs.PeekNext());
			Assert.AreEqual("Hello, World!", cs.ReadLine());
			Assert.AreEqual("你好，世界！", cs.ReadLine());
			Assert.IsEmpty(cs.ReadLine());
			Assert.AreEqual("Chào thế giới!", cs.ReadLine());
			Assert.IsEmpty(cs.ReadLine());
			Assert.IsEmpty(cs.ReadLine());

			Assert.AreEqual("foo", cs.ReadWhile((c) => Char.IsLetter((char)c)));
			Assert.AreEqual("123", cs.ReadWhile((c) => Char.IsDigit((char)c)));
			cs.SkipCharacter(' ');
			Assert.AreEqual('b', cs.PeekNext());
			cs.SkipNext(); // b
			Assert.AreEqual('a', cs.PeekNext());
			cs.SkipCharacter('a');
			Assert.AreEqual('r', cs.ReadNext());
			cs.SkipNext(); // \n

			Assert.AreEqual("こんにちは世界！", cs.ReadLine());

			// read 1 line
			Assert.AreEqual(1, cs.SkipWhile(CharStream.IsNL));

			// Read mix between ASCII (dsl) and unicode (locale)
			cs.SkipCharacter('<');
			Assert.AreEqual("hello", cs.ReadWhile(char.IsLetter));
			cs.SkipString(" \"");
			Assert.AreEqual("你好", cs.ReadWhile((c) => c != '"'));
			Assert.AreEqual("\">", cs.ReadLine());

			// empty seperator line
			Assert.IsEmpty(cs.ReadLine());

			// making sure we're still correct
			Assert.AreEqual('0', cs.PeekNext());
			Assert.AreEqual('0', cs.PeekNext());

			int pos = 0;
			while(char.IsDigit(cs.PeekNext())) {
				cs.SkipCharacter((pos++).ToString()[0]);
				if(cs.PeekNext() != ',')
					break; // early exit
				cs.SkipCharacter(',');
			}
			Assert.AreEqual('a', cs.PeekNext());
			cs.SkipBlock(2); // a,
			while(char.IsDigit(cs.PeekNext())) {
				cs.SkipCharacter((pos++).ToString()[0]);
				if(cs.PeekNext() != ',')
					break; // early exit
				cs.SkipCharacter(',');
			}
			Assert.IsEmpty(cs.ReadLine());
			Assert.IsEmpty(cs.ReadLine());

			cs.SkipString("[[");
			Assert.AreEqual("end", cs.ReadBlock(3));
			cs.SkipString("]]");
			Assert.IsEmpty(cs.ReadLine());

			Assert.AreEqual("\nor is it?\n", cs.ReadUntilEnd());

			Assert.IsTrue(cs.EndOfStream());
		}

		[Test()]
		public void AllKindOfNLChars()
		{
			var cs = NCS("\none\rtwo\nthree\r\nfour\r\rsix\nseven\r\neight\n");
			
			Assert.IsEmpty(cs.ReadLine());
			Assert.AreEqual("one", cs.ReadLine());
			Assert.AreEqual("two", cs.ReadLine());
			Assert.AreEqual("three", cs.ReadLine());
			Assert.AreEqual("four", cs.ReadLine());
			Assert.IsEmpty(cs.ReadLine());
			Assert.AreEqual("six", cs.ReadLine());
			Assert.AreEqual("seven", cs.ReadLine());
			Assert.AreEqual("eight", cs.ReadLine());
			Assert.IsTrue(cs.EndOfStream());
		}

		[Test()]
		public void SkipTests()
		{
			var cs = NCS(" a   b12345678bde hello world!1");
			cs.SkipNext();
			cs.SkipCharacter('a');
			cs.SkipNext();
			cs.SkipBlock(2);
			cs.SkipCharacter('b');
			Assert.AreEqual(8, cs.SkipWhile(char.IsDigit));
			Assert.AreEqual('b', cs.PeekNext());
			Assert.AreEqual(3, cs.SkipWhile(char.IsLetter));
			Assert.AreEqual(13, cs.SkipUntil(char.IsDigit));
			Assert.AreEqual('1', cs.PeekNext());
			Assert.AreEqual(1, cs.SkipWhile((c) => true));
			Assert.IsTrue(cs.EndOfStream());
			Assert.AreEqual(0, cs.SkipUntil((c) => false));
			Assert.IsTrue(cs.EndOfStream());
		}

		[Test()]
		public void StreamBufferTests()
		{
			var cs = NCS("abcde\r\nfghijk");
			int pos = cs.Position;
			Assert.AreEqual(0, pos);
			for(int i = 0; i < 5; ++i) {
				Assert.AreEqual("abcde", cs.ReadLine());
				Assert.AreEqual(7, cs.Position);
				cs.Rewind(pos);
				Assert.AreEqual(0, cs.Position);
			}

			Assert.AreEqual("abcde", cs.ReadLine());
			Assert.AreEqual("fghijk", cs.ReadLine());
		}

		[Test()]
		public void ExceptionTests()
		{
			// EOF while character was expected
			Throws(() => NCS("").ReadNext());
			// wrong character
			Throws(() => NCS("a").SkipCharacter('b'));
		}

		private delegate void ThrowFunction();
		private static void Throws(ThrowFunction f)
		{
			try {
				f();
				Assert.IsFalse(true, "does not throw, while it was expected");
			} catch(ParseException e) {
				Assert.IsNotNull(e);
			}
		}

		public static CharStream NCS(string buffer)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));
			return new CharStream(new StreamReader(stream));
		}
	}
}
