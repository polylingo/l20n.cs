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
			stream = NCS("# a comment in expected form\n");
			Assert.IsTrue(Comment.PeekAndParse(stream, fctx, out node));
			Assert.IsNotNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
			// This will also read everything, but as it's partial, it will be discarded
			stream = NCS("# a comment in expected form\n");
			Assert.IsTrue(Comment.PeekAndParse(stream, pctx, out node));
			Assert.IsNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
				
			// this will fail
			Assert.IsFalse(Comment.PeekAndParse(NCS("as it is not a comment\n"), pctx, out node));
			Assert.IsNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
			// this will also fail
			Assert.IsFalse(Comment.PeekAndParse(NCS("as # it is still not a comment\n"), pctx, out node));
			Assert.IsNull(node);
			Assert.IsEmpty(stream.ReadUntilEnd());
				
			// The Comment parser will read the entire stream
			// once it detirmined it's a legal comment
			stream = NCS("# a comment in expected form\n# new comment\n");
			Assert.IsTrue(Comment.PeekAndParse(stream, fctx, out node));
			Assert.IsNotNull(node);
			Assert.AreEqual("# new comment\n", stream.ReadUntilEnd());
		}

		[Test()]
		public void KeywordTests()
		{
			// a normal (and best case example)
			Assert.IsNotNull(Keyword.Parse(NCS("hello")));
			
			// other legal (but not always great) examples
			Assert.IsNotNull(Keyword.Parse(NCS("this is valid")));
			Assert.IsNotNull(Keyword.Parse(NCS("this_is_also_valid")));
			Assert.IsNotNull(Keyword.Parse(NCS("this-is-also-valid")));
			Assert.IsNotNull(Keyword.Parse(NCS("Could be a sentence.")));
			Assert.IsNotNull(Keyword.Parse(NCS("Or a question?")));
			Assert.IsNotNull(Keyword.Parse(NCS("Room 42")));
			
			// bad examples
			Throws(() => Keyword.Parse(NCS(""))); // cannot be empty
			Throws(() => Keyword.Parse(NCS("4 cannot start with a number")));
			Throws(() => Keyword.Parse(NCS("@ is not allowed")));
			Throws(() => Keyword.Parse(NCS("# is not allowed")));
			Throws(() => Keyword.Parse(NCS(" cannot start with space")));
		}

		[Test()]
		public void SectionFullASTTests()
		{
			L20n.FTL.AST.INode node;
			
			var ctx = new L20n.FTL.Parsers.Context(Context.ASTTypes.Full);
			
			// a section starts with '[['
			Assert.IsFalse(Section.PeekAndParse(NCS("not a section"), ctx, out node));
			Throws(() => Section.PeekAndParse(NCS("[not a section either]"), ctx, out node));
			Assert.IsTrue(Section.PeekAndParse(NCS("[[ a section ]]\n"),ctx,  out node));
			Throws(() => Section.PeekAndParse(NCS("[[ needs to end with double brackets"), ctx, out node));
			Throws(() => Section.PeekAndParse(NCS("[[ needs to end with double brackets ]"), ctx, out node));
			Throws(() => Section.PeekAndParse(NCS("[[ needs to have newline char at the end ]]"), ctx, out node));
		}

		[Test()]
		public void IdentifierTests()
		{
			// a normal (and best case example)
			Assert.IsNotNull(Identifier.Parse(NCS("hello")));
			
			// other legal (but not always great) examples
			Assert.IsNotNull(Identifier.Parse(NCS("this is valid")));
			Assert.IsNotNull(Identifier.Parse(NCS("this_is_also_valid")));
			Assert.IsNotNull(Identifier.Parse(NCS("this-is-also-valid")));
			Assert.IsNotNull(Identifier.Parse(NCS("Could be a sentence.")));
			Assert.IsNotNull(Identifier.Parse(NCS("Or a question?")));
			Assert.IsNotNull(Identifier.Parse(NCS("Room 42")));
			Assert.IsNotNull(Identifier.Parse(NCS("?")));
			Assert.IsNotNull(Identifier.Parse(NCS(".-?_???")));
			
			// bad examples
			Throws(() => Identifier.Parse(NCS(""))); // cannot be empty
			Throws(() => Identifier.Parse(NCS("4 cannot start with a number")));
			Throws(() => Identifier.Parse(NCS("@ is not allowed")));
			Throws(() => Identifier.Parse(NCS("# is not allowed")));
			Throws(() => Identifier.Parse(NCS("# is not allowed")));
			Throws(() => Identifier.Parse(NCS(" cannot start with space")));
		}

		[Test()]
		public void BuiltinTests()
		{
			// a normal (and best case example)
			Assert.IsNotNull(Builtin.Parse(NCS("NUMBER")));
			
			// other legal (but not always great) examples
			Assert.IsNotNull(Builtin.Parse(NCS("SOME_BUILT-IN")));
			Assert.IsNotNull(Builtin.Parse(NCS("?-._A-Z")));
			
			// bad examples
			Throws(() => Builtin.Parse(NCS(""))); // cannot be empty
			Throws(() => Builtin.Parse(NCS("4 cannot start with a number")));
			Throws(() => Builtin.Parse(NCS("@ is not allowed")));
			Throws(() => Builtin.Parse(NCS("# is not allowed")));
			Throws(() => Builtin.Parse(NCS("# is not allowed")));
			Throws(() => Builtin.Parse(NCS(" cannot start with space")));
			Throws(() => Builtin.Parse(NCS("aNOPE"))); // cannot contain lowercase letters
		}

		private void checkValidNumber(string input, string expected, string rest = "")
		{
			var cs = NCS(input);
			var number = Number.Parse(cs);
			Assert.IsNotNull(number);
			Assert.AreEqual(rest, cs.ReadUntilEnd());

			Writer writer = new Writer(new StringWriter());
			number.Serialize(writer);
			Assert.AreEqual(expected, writer.ToString());
		}

		[Test()]
		public void NumberTests()
		{
			// legal examples
			checkValidNumber("42", "42");
			checkValidNumber("123.456", "123.456");
			
			// bad examples that do not make it throw an exception,
			// but just stop parsing instead
			checkValidNumber("42,00", "42", ",00");
			
			// bad examples
			Throws(() => Number.Parse(NCS("42."))); // no digits behind the point given
			Throws(() => Number.Parse(NCS("-42"))); // '-' not allowed
			Throws(() => Number.Parse(NCS("+42"))); // '+' not allowed
			Throws(() => Number.Parse(NCS("hello"))); // only numbers allowed
		}

		[Test()]
		public void VariableTests()
		{
			// As long as it's an identifier prefixed with '$' it's fine
			Assert.IsNotNull(Variable.Parse(NCS("$hello")));
			Assert.IsNotNull(Variable.Parse(NCS("$Whatever")));
			
			// otherwise we get an exception
			Throws(() => Variable.Parse(NCS("nope"))); // no '$' prefix

			Throws(() => Variable.Parse(NCS("$4"))); // illegal identifier

			// check on Identifier to know more about what
			// kind of identifier is legal and what not
		}

		[Test()]
		public void MemberExpressionTests()
		{
			Assert.IsTrue(MemberExpression.Peek(NCS("[")));
			Assert.IsFalse(MemberExpression.Peek(NCS("foo")));
			
			var id = Identifier.Parse(NCS("foo"));
			
			// MemberExpressionParsing starts from the '['
			Assert.IsNotNull(MemberExpression.Parse(NCS("[bar]"), id));
			Assert.IsNotNull(MemberExpression.Parse(NCS("[this_is-ok?42]"), id));
			
			// otherwise we get an exception
			Throws(() => MemberExpression.Parse(NCS("nope"), id)); // no '[' prefix
			Throws(() => MemberExpression.Parse(NCS("[nope"), id)); // no ']' postfix
			Throws(() => MemberExpression.Parse(NCS("[42]"), id)); // illegal keyword
		}

		[Test()]
		public void AttributeTests()
		{
			// good examples
			Assert.IsNotNull(Memberkey.Parse(NCS("bar")));
			Assert.IsNotNull(Memberkey.Parse(NCS("foo/bar")));
			Assert.IsNotNull(Memberkey.Parse(NCS("42")));
			Assert.IsNotNull(Memberkey.Parse(NCS("42.0")));

			// bad examples
			Throws(() => Memberkey.Parse(NCS("foo/42")));

			// check identifier-, keyword- and number- tests for more info
		}

		private void checkValidArgument<T>(string input, string rest = "")
		{
			var cs = NCS(input);
			var argument = Argument.Parse(cs);
			Assert.IsNotNull(argument);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(T), argument.GetType());
		}
		
		[Test()]
		public void ArgumentTests()
		{
			// just some examples,
			// for more good/bad examples of each case,
			// check the tests for that case
			
			// Identifier
			checkValidArgument<L20n.FTL.AST.StringPrimitive>("Foo");
			checkValidExpression<L20n.FTL.AST.StringPrimitive>("boo909");
			
			// MemberExpression
			checkValidArgument<L20n.FTL.AST.MemberExpression>("foo[bar]");
			checkValidArgument<L20n.FTL.AST.MemberExpression>("Foo09?[bar09?]");
			
			// CallExpression
			checkValidArgument<L20n.FTL.AST.CallExpression>("FOO()");
			checkValidArgument<L20n.FTL.AST.CallExpression>("FOO(DROP(2, baz))");
			
			// Variable
			checkValidArgument<L20n.FTL.AST.Variable>("$ok");
			checkValidArgument<L20n.FTL.AST.Variable>("$Variable");
			
			// QuotedPattern
			checkValidArgument<L20n.FTL.AST.Pattern>("\"value\"");
			checkValidArgument<L20n.FTL.AST.Pattern>("\"{ \"{ $variable }\" }\"");
			
			// Number
			checkValidArgument<L20n.FTL.AST.Number>("42");
			checkValidArgument<L20n.FTL.AST.Number>("123.456");

			// KeywordArgument
			checkValidArgument<L20n.FTL.AST.KeywordArgument>("key = \"value\"");
			checkValidArgument<L20n.FTL.AST.KeywordArgument>("key = \"{ $variable }\"");

			// should not be empty
			Throws(() => Argument.Parse(NCS("")));
		}

		[Test()]
		public void CallExpressionTests()
		{
			var builtin = Builtin.Parse(NCS("FOO"));

			// good examples
			Assert.IsNotNull(CallExpression.Parse(NCS("()"), builtin));
			Assert.IsNotNull(CallExpression.Parse(NCS("(hello)"), builtin));
			Assert.IsNotNull(CallExpression.Parse(NCS("(hello, world)"), builtin));
			Assert.IsNotNull(CallExpression.Parse(NCS("(hello  ,  world)"), builtin));
			Assert.IsNotNull(CallExpression.Parse(NCS("(hello,world, 42)"), builtin));
			Assert.IsNotNull(CallExpression.Parse(NCS("(glen, 42, member[test], $ok, FOO(bar))"), builtin));

			// bad examples
			// should not be empty
			Throws(() => Argument.Parse(NCS("")));
		}

		private void checkValidExpression<T>(string input, string rest = "")
		{
			var cs = NCS(input);
			var expression = Expresson.Parse(cs);
			Assert.IsNotNull(expression);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(T), expression.GetType());
		}

		[Test()]
		public void ExpressionTests()
		{
			// just some examples,
			// for more good/bad examples of each case,
			// check the tests for that case

			// Identifier
			checkValidExpression<L20n.FTL.AST.StringPrimitive>("Foo");
			checkValidExpression<L20n.FTL.AST.StringPrimitive>("boo909");

			// MemberExpression
			checkValidExpression<L20n.FTL.AST.MemberExpression>("foo[bar]");
			checkValidExpression<L20n.FTL.AST.MemberExpression>("Foo09?[bar09?]");
			
			// CallExpression
			checkValidExpression<L20n.FTL.AST.CallExpression>("FOO()");
			checkValidExpression<L20n.FTL.AST.CallExpression>("FOO(DROP(2, baz))");
			
			// Variable
			checkValidExpression<L20n.FTL.AST.Variable>("$ok");
			checkValidExpression<L20n.FTL.AST.Variable>("$Variable");

			// QuotedPattern
			checkValidArgument<L20n.FTL.AST.Pattern>("\"value\"");
			checkValidArgument<L20n.FTL.AST.Pattern>("\"{ \"{ $variable }\" }\"");

			// Number
			checkValidExpression<L20n.FTL.AST.Number>("42");
			checkValidExpression<L20n.FTL.AST.Number>("123.456");

			// should not be empty
			Throws(() => Argument.Parse(NCS("")));
		}

		[Test()]
		public void UnquotedTextTests()	
		{
			Assert.IsNotNull(AnyText.ParseUnquoted(NCS("this is fine")));
			Assert.IsNotNull(AnyText.ParseUnquoted(NCS("can be almost anything\nanother line")));
			Assert.IsNotNull(AnyText.ParseUnquoted(NCS("curly brackets to have be escaped like this: \\{")));

			CharStream cs = NCS("otherwise {it will stop");
			Assert.IsNotNull(AnyText.ParseUnquoted(cs));
			Assert.AreEqual("{it will stop", cs.ReadUntilEnd());
		}
		
		[Test()]
		public void QuotedTextTests()	
		{
			Assert.IsNotNull(AnyText.ParseQuoted(NCS("this is fine")));
			Assert.IsNotNull(AnyText.ParseQuoted(NCS("can be almost anything\nanother line")));
			Assert.IsNotNull(AnyText.ParseQuoted(NCS("curly brackets to have be escaped like this: \\{")));
			Assert.IsNotNull(AnyText.ParseQuoted(NCS("a quote has to be escaped like this: \\\"")));
			
			CharStream cs = NCS("otherwise {it will stop");
			Assert.IsNotNull(AnyText.ParseQuoted(cs));
			Assert.AreEqual("{it will stop", cs.ReadUntilEnd());

			cs = NCS("otherwise \"it will stop");
			Assert.IsNotNull(AnyText.ParseQuoted(cs));
			Assert.AreEqual("\"it will stop", cs.ReadUntilEnd());
		}
		
		[Test()]
		public void BlockTextTests()	
		{
			// TODO : Fix this buggy parsing
			// Also... we have a potential problem with block text,
			// as a block text consists out of `| <unquoted-pattern>`
			// and an unquoted-pattern can be a block, it means that
			// a block-text of 3 lines, is 3 unquoted-patterns deep, rather than simply returning early
			// this shouldn't really happen I think, as it is asking for trouble...
			// need to find a way to fix this, without breaking l20n spec intentions.
			/*
			L20n.FTL.AST.INode result;
			// good examples
			Assert.IsTrue(AnyText.PeekAndParseBlock(NCS(@"
				| this blocktext
				| is fine"), out result));
			Assert.IsTrue(AnyText.PeekAndParseBlock(NCS(@"
| this blocktext is also fine"), out result));

			// bad examples
			// newline required
			Assert.IsFalse(AnyText.PeekAndParseBlock(NCS(""), out result));
			// | required
			Assert.IsFalse(AnyText.PeekAndParseBlock(NCS(@"
"), out result));*/
		}

		// BodyParser: TODO:
		// Make sure that FTL files that end with no empty new line work as well

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