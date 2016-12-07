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
	public sealed class BufferStreamReaderTests
	{
		[Test()]
		public void StreamBufferTests()
		{
			var cs = NBSR("a\r\nc\ne\rf");

			int pos = cs.GetBufferPosition();
			Assert.AreEqual(0, pos);

			Assert.AreEqual('a', cs.PeekNext());
			Assert.AreEqual(pos, cs.GetBufferPosition());
			Assert.AreEqual('a', cs.ReadNext());

			cs.RewindBuffer(pos);
			Assert.AreEqual('a', cs.PeekNext());
			Assert.AreEqual('a', cs.ReadNext());

			pos = cs.GetBufferPosition();
			Assert.AreEqual(1, pos);

			Assert.AreEqual('\r', cs.PeekNext());
			Assert.AreEqual(pos, cs.GetBufferPosition());
			Assert.AreEqual('\r', cs.ReadNext());
			
			cs.RewindBuffer(pos);
			Assert.AreEqual(1, pos);

			Assert.AreEqual('\r', cs.PeekNext());
			Assert.AreEqual('\r', cs.ReadNext());
			
			pos = cs.GetBufferPosition();
			Assert.AreEqual(2, pos);

			Assert.AreEqual('\n', cs.PeekNext());
			Assert.AreEqual(pos, cs.GetBufferPosition());
			Assert.AreEqual('\n', cs.ReadNext());
			
			pos = cs.GetBufferPosition();
			Assert.AreEqual(3, pos);

			Assert.AreEqual('c', cs.PeekNext());
			cs.FlushBuffer();
			pos = cs.GetBufferPosition();
			Assert.AreEqual(0, pos);
			Assert.AreEqual('c', cs.PeekNext());
			cs.RewindBuffer(pos);
			Assert.AreEqual('c', cs.PeekNext());
			Assert.AreEqual('c', cs.PeekNext());

			pos = cs.GetBufferPosition();
			Assert.AreEqual(0, pos);

			for(int i = 0; i < 5; ++i) {
				Assert.AreEqual('c', cs.ReadNext());
				Assert.AreEqual('\n', cs.ReadNext());
				Assert.AreEqual('e', cs.ReadNext());
				Assert.AreEqual('\r', cs.ReadNext());
				Assert.AreEqual('f', cs.ReadNext());

				pos = cs.GetBufferPosition();
				Assert.AreEqual(5, pos);
				
				Assert.AreEqual(0, cs.PeekNext());

				cs.RewindBuffer(0);
				
				pos = cs.GetBufferPosition();
				Assert.AreEqual(0, pos);
			}
			
			Assert.AreEqual('c', cs.PeekNext());
			Assert.AreEqual('c', cs.ReadNext());
			Assert.AreEqual('\n', cs.PeekNext());
			Assert.AreEqual('\n', cs.ReadNext());
			Assert.AreEqual('e', cs.PeekNext());
			Assert.AreEqual('e', cs.ReadNext());
			Assert.AreEqual('\r', cs.PeekNext());
			Assert.AreEqual('\r', cs.ReadNext());
			Assert.AreEqual('f', cs.PeekNext());
			Assert.AreEqual('f', cs.ReadNext());

			Assert.AreEqual('\0', cs.PeekNext());
		}

		private static BufferedStreamReader NBSR(string buffer)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));
			return new BufferedStreamReader(new StreamReader(stream));
		}
	}
}