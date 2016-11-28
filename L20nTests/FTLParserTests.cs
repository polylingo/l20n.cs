// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using L20n.IO;
using L20n.Exceptions;
using L20n.FTL.Parsers;

namespace L20nTests
{
	[TestFixture()]
	/// <summary>
	/// Tests for all the individual parsers.
	/// The l20n parser is a parser of parsers, so if all the small pieces work,
	/// there is a very high chance that the final result is OK.
	/// </summary>
	public class FTLParserTests
	{
		[Test()]
		public void WhiteSpaceTests()
		{
			var stream = NCS("   \t\t  ");
			
			// This will read everything
			WhiteSpace.Skip(stream);
			// This will not read anything, but it's optional
			// so it will not give an exception
			Assert.IsFalse(WhiteSpace.PeekAndSkip(stream));
			// when trying to skip it will however give an exception
			Throws(() => WhiteSpace.Skip(stream));
			
			
			stream = NCS("   a <- foo");
			
			// This will read until 'a'
			WhiteSpace.Skip(stream);
			Assert.IsFalse(WhiteSpace.PeekAndSkip(stream));
			Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
		}
		
		[Test()]
		public void NewLineTests()
		{
			var stream = NCS("\n\r\n\n\n");
			
			// This will read everything
			NewLine.Skip(stream);
			// This will fail as it's not optional
			Throws(() => NewLine.Skip(stream));
			// When it's optional it will simply return false
			Assert.IsFalse(NewLine.PeekAndSkip(stream));
			
			
			stream = NCS("\n\r\n\n\ra <- foo");
			
			// This will read until 'a'
			Assert.IsTrue(NewLine.PeekAndSkip(stream));
			// This will fail as it's not optional
			Throws(() => NewLine.Skip(stream));
			Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
		}

		[Test()]
		public void CommentTests()
		{
			CharStream stream;

			var fctx = new Context(Context.ASTTypes.Full);
			var pctx = new Context(Context.ASTTypes.Partial);

			L20n.FTL.AST.INode node;
				
			// This will read everything
			stream = NCS("# a comment in expected form");
			Assert.IsTrue(Comment.PeekAndParse(stream, fctx, out node));
			Assert.IsNotNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
			// This will also read everything, but as it's partial, it will be discarded
			stream = NCS("# a comment in expected form");
			Assert.IsTrue(Comment.PeekAndParse(stream, pctx, out node));
			Assert.IsNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
				
			// this will fail
			Assert.IsFalse(Comment.PeekAndParse(NCS("as it is not a comment"), pctx, out node));
			Assert.IsNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
			// this will also fail
			Assert.IsFalse(Comment.PeekAndParse(NCS("as # it is still not a comment"), pctx, out node));
			Assert.IsNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
				
			// The Comment parser will read the entire stream
			// once it detirmined it's a legal comment
			stream = NCS("# a comment in expected form\n# new comment");
			Assert.IsTrue(Comment.PeekAndParse(stream, fctx, out node));
			Assert.IsNotNull(node);
			Assert.AreEqual("\n# new comment", stream.ReadUntilEnd());
		}

		public static CharStream NCS(string buffer)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));
			return new CharStream(new StreamReader(stream));
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
	}
}