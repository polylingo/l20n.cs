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
			var cs = new CharStream("../../resources/io/Empty.txt");
			Assert.AreEqual("L1:1", cs.ComputeDetailedPosition());
			Assert.IsTrue(cs.EndOfStream());
		}

		[Test()]
		public void CharStreamFile()
		{
			var cs = new CharStream("../../resources/io/CharStream.txt");
			Assert.AreEqual("L1:1", cs.ComputeDetailedPosition());

			Assert.AreEqual('H', cs.PeekNext());
			Assert.AreEqual("Hello, World!", cs.ReadLine());
			Assert.AreEqual("L2:1", cs.ComputeDetailedPosition());
			Assert.AreEqual("你好，世界！", cs.ReadLine());
			Assert.AreEqual("L3:1", cs.ComputeDetailedPosition());
			Assert.IsEmpty(cs.ReadLine());
			Assert.AreEqual("L4:1", cs.ComputeDetailedPosition());
			Assert.AreEqual("Chào thế giới!", cs.ReadLine());
			Assert.AreEqual("L5:1", cs.ComputeDetailedPosition());
			Assert.IsEmpty(cs.ReadLine());
			Assert.AreEqual("L6:1", cs.ComputeDetailedPosition());
			Assert.IsEmpty(cs.ReadLine());
			Assert.AreEqual("L7:1", cs.ComputeDetailedPosition());

			Assert.AreEqual("foo", cs.ReadWhile((c) => Char.IsLetter((char)c)));
			Assert.AreEqual("L7:4", cs.ComputeDetailedPosition());
			Assert.AreEqual("123", cs.ReadWhile((c) => Char.IsDigit((char)c)));
			Assert.AreEqual("L7:7", cs.ComputeDetailedPosition());
			cs.SkipCharacter(' ');
			Assert.AreEqual('b', cs.PeekNext());
			Assert.AreEqual("L7:8", cs.ComputeDetailedPosition());
			cs.SkipNext(); // b
			Assert.AreEqual('a', cs.PeekNext());
			Assert.AreEqual("L7:9", cs.ComputeDetailedPosition());
			cs.SkipCharacter('a');
			Assert.AreEqual("L7:10", cs.ComputeDetailedPosition());
			Assert.AreEqual('r', cs.ReadNext());
			Assert.AreEqual("L7:11", cs.ComputeDetailedPosition());
			cs.SkipNext(); // \n
			Assert.AreEqual("L8:1", cs.ComputeDetailedPosition());

			Assert.AreEqual("こんにちは世界！", cs.ReadLine());
			Assert.AreEqual("L9:1", cs.ComputeDetailedPosition());

			// read 1 line
			Assert.AreEqual(1, cs.SkipWhile((c) => c == CharStream.NL));
			Assert.AreEqual("L10:1", cs.ComputeDetailedPosition());

			// Read mix between ASCII (dsl) and unicode (locale)
			cs.SkipCharacter('<');
			Assert.AreEqual("L10:2", cs.ComputeDetailedPosition());
			Assert.AreEqual("hello", cs.ReadWhile(char.IsLetter));
			Assert.AreEqual("L10:7", cs.ComputeDetailedPosition());
			cs.SkipString(" \"");
			Assert.AreEqual("L10:9", cs.ComputeDetailedPosition());
			Assert.AreEqual("你好", cs.ReadWhile((c) => c != '"'));
			Assert.AreEqual("L10:11", cs.ComputeDetailedPosition());
			Assert.AreEqual("\">", cs.ReadLine());
			Assert.AreEqual("L11:1", cs.ComputeDetailedPosition());

			// empty seperator line
			Assert.IsEmpty(cs.ReadLine());

			// making sure we're still correct
			Assert.AreEqual('0', cs.PeekNext());
			Assert.AreEqual('0', cs.PeekNext());
			Assert.AreEqual("L12:1", cs.ComputeDetailedPosition());

			int pos = 0;
			while(char.IsDigit(cs.PeekNext())) {
				cs.SkipCharacter((pos++).ToString()[0]);
				if(cs.PeekNext() != ',')
					break; // early exit
				cs.SkipCharacter(',');
			}
			Assert.AreEqual("L12:11", cs.ComputeDetailedPosition());
			Assert.AreEqual('a', cs.PeekNext());
			cs.SkipBlock(2); // a,
			Assert.AreEqual("L12:13", cs.ComputeDetailedPosition());
			while(char.IsDigit(cs.PeekNext())) {
				cs.SkipCharacter((pos++).ToString()[0]);
				if(cs.PeekNext() != ',')
					break; // early exit
				cs.SkipCharacter(',');
			}
			Assert.AreEqual("L12:22", cs.ComputeDetailedPosition());
			Assert.IsEmpty(cs.ReadLine());
			Assert.IsEmpty(cs.ReadLine());

			cs.SkipString("[[");
			Assert.AreEqual("L14:3", cs.ComputeDetailedPosition());
			Assert.AreEqual("end", cs.ReadBlock(3));
			Assert.AreEqual("L14:6", cs.ComputeDetailedPosition());
			cs.SkipString("]]");
			Assert.AreEqual("L14:8", cs.ComputeDetailedPosition());
			Assert.IsEmpty(cs.ReadLine());

			Assert.AreEqual("\nor is it?\n", cs.ReadUntilEnd());
			Assert.AreEqual("L17:1", cs.ComputeDetailedPosition());

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
		public void ExceptionTests()
		{
			try {
				NCS("a").SkipCharacter('b');
				Assert.IsFalse(true, "should not be reached");
			} catch(Exception e) {
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
