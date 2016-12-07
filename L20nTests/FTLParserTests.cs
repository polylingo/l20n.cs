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
		[Test(), Timeout(2000)]
		public void WhiteSpaceTests()
		{
			var stream = NCS("   \t\t  ");
			
			// This will read everything
			WhiteSpace.Parse(stream);
			// This will not read anything, but it's optional
			// so it will not give an exception
			WhiteSpace.Parse(stream);
			
			
			stream = NCS("   a <- foo");
			
			// This will read until 'a'
			WhiteSpace.Parse(stream);
			WhiteSpace.Parse(stream);
			Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
		}
		
		[Test(), Timeout(2000)]
		public void NewLineTests()
		{
			var stream = NCS("\n\r\n\n\n");
			
			// This will read everything
			NewLine.Parse(stream);
			// This will fail as it's not optional
			Throws(() => NewLine.Parse(stream));
			
			
			stream = NCS("\n\r\n\n\ra <- foo");
			
			// This will read until 'a'
			NewLine.Parse(stream);
			// This will fail as it's not optional
			Throws(() => NewLine.Parse(stream));
			Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
		}

		[Test(), Timeout(2000)]
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
			stream = NCS("# a comment in expected form\n");
			Assert.IsTrue(Comment.PeekAndParse(stream, pctx, out node));
			Assert.IsNull(node);
			Assert.AreEqual("\n", stream.ReadUntilEnd());
				
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
			stream = NCS("# a comment in expected form\n# new comment\n");
			Assert.IsTrue(Comment.PeekAndParse(stream, fctx, out node));
			Assert.IsNotNull(node);
			stream.SkipNext(); // skip newline
			Assert.IsTrue(Comment.PeekAndParse(stream, fctx, out node));
			Assert.IsNotNull(node);
			Assert.AreEqual("\n", stream.ReadUntilEnd());
		}

		[Test(), Timeout(2000)]
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

		[Test(), Timeout(2000)]
		public void SectionFullASTTests()
		{
			L20n.FTL.AST.INode node;
			
			var ctx = new L20n.FTL.Parsers.Context(Context.ASTTypes.Full);
			
			// a section starts with '[['
			Assert.IsFalse(Section.PeekAndParse(NCS("not a section"), ctx, out node));
			Throws(() => Section.PeekAndParse(NCS("[not a section either]"), ctx, out node));
			Assert.IsTrue(Section.PeekAndParse(NCS("[[ a section ]]"),ctx,  out node));
			Throws(() => Section.PeekAndParse(NCS("[[ needs to end with double brackets"), ctx, out node));
			Throws(() => Section.PeekAndParse(NCS("[[ needs to end with double brackets ]"), ctx, out node));
		}

		[Test(), Timeout(2000)]
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

		[Test(), Timeout(2000)]
		public void BuiltinTests()
		{
			// a normal (and best case example)
			Assert.IsNotNull(Builtin.Parse(NCS("NUMBER")));
			Assert.IsTrue(Builtin.IsValid("NUMBER"));
			
			// other legal (but not always great) examples
			Assert.IsNotNull(Builtin.Parse(NCS("SOME_BUILT-IN")));
			Assert.IsNotNull(Builtin.Parse(NCS("?-._A-Z")));

			Assert.IsTrue(Builtin.IsValid("SOME_BUILT-IN"));
			Assert.IsTrue(Builtin.IsValid("?-._A-Z"));
			
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

		[Test(), Timeout(2000)]
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

		[Test(), Timeout(2000)]
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

		[Test(), Timeout(2000)]
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

		[Test(), Timeout(2000)]
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
		
		[Test(), Timeout(2000)]
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

		private void CheckValidUnquotedPattern(string input, string rest = "")
		{
			var cs = NCS(input);
			var argument = Pattern.ParseUnquoted(cs);
			Assert.IsNotNull(argument);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(L20n.FTL.AST.Pattern), argument.GetType());
		}

		[Test(), Timeout(2000)]
		public void ParseUnquotedPattern()
		{
			// block-text
			CheckValidUnquotedPattern(@"
				| this blocktext
				| is fine");

			// Placeable: Expression
			CheckValidUnquotedPattern("{ Foo }");
			CheckValidUnquotedPattern("{ foo[bar] }");
			CheckValidUnquotedPattern("{ $variable }");
			CheckValidUnquotedPattern("{ \"value\" }");
			CheckValidUnquotedPattern("{ 42 }");
			
			// Placeable: Expressions
			CheckValidUnquotedPattern("{ Foo, foo[bar], $variable, \"value\", 42 }");
			
			// Placeable: SelectExpression
			CheckValidUnquotedPattern(@"{ $count ->
				[0] There are no items :(
				[1] There is one item :|
				*[other] { $variable ->
					[a] something related to a
					[b] something related to b
				}
			}");

			// unquoted-text
			CheckValidUnquotedPattern("hello my dear child");

			// combination of unquoted-text and placeable
			CheckValidUnquotedPattern("Hello, { $name }!");

			// combination of block-text and placeable
			CheckValidUnquotedPattern(@"
				| Hello, { $name },
				| how are you today, on { DAY($date) } { MONTH($date) }?");
		}

		[Test(), Timeout(2000)]
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

		[Test(), Timeout(2000)]
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

		private void CheckValidPlaceable(string input, string rest = "")
		{
			var cs = NCS(input);
			L20n.FTL.AST.INode result;
			Assert.IsTrue(Placeable.PeekAndParse(cs, out result));
			Assert.IsNotNull(result);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(L20n.FTL.AST.Placeable), result.GetType());
		}
		
		[Test(), Timeout(2000)]
		public void PlaceableTests()
		{
			// Expression
			CheckValidPlaceable("{ Foo }");
			CheckValidPlaceable("{ foo[bar] }");
			CheckValidPlaceable("{ $variable }");
			CheckValidPlaceable("{ \"value\" }");
			CheckValidPlaceable("{ 42 }");

			// Expressions
			CheckValidPlaceable("{ Foo, foo[bar], $variable, \"value\", 42 }");

			// SelectExpression
			CheckValidPlaceable(@"{ $count ->
				[0] There are no items :(
				[1] There is one item :|
				*[other] { $variable ->
					[a] something related to a
					[b] something related to b
				}
			}");
			
			// for more examples check the different types of expressions
		}

		[Test(), Timeout(2000)]
		public void UnquotedTextTests()	
		{
			Assert.IsNotNull(AnyText.ParseUnquoted(NCS("this is fine")));
			Assert.IsNotNull(AnyText.ParseUnquoted(NCS("can be almost anything\nanother line")));
			Assert.IsNotNull(AnyText.ParseUnquoted(NCS("curly brackets to have be escaped like this: \\{")));

			CharStream cs = NCS("otherwise {it will stop");
			Assert.IsNotNull(AnyText.ParseUnquoted(cs));
			Assert.AreEqual("{it will stop", cs.ReadUntilEnd());
		}
		
		[Test(), Timeout(2000)]
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
			L20n.FTL.AST.INode result;
			// good examples
			Assert.IsTrue(AnyText.PeekAndParseBlock(NCS(@"
				| this blocktext
				| is fine"), out result));
			var cs = NCS(@"
| this blocktext is also fine
	hello");
			Assert.IsTrue(AnyText.PeekAndParseBlock(cs, out result));
			Assert.AreEqual(@"
	hello", cs.ReadUntilEnd());

			// bad examples
			// newline required
			Assert.IsFalse(AnyText.PeekAndParseBlock(NCS(""), out result));
			// | required
			Assert.IsFalse(AnyText.PeekAndParseBlock(NCS(@"
"), out result));
		}

		private void checkValidMemberkey<T>(string input, string rest = "")
		{
			var cs = NCS(input);
			var memberKey = Memberkey.Parse(cs);
			Assert.IsNotNull(memberKey);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(T), memberKey.GetType());
		}

		[Test(), Timeout(2000)]
		public void MemberKeyTests()
		{
			// the three possible MemberKeys
			checkValidMemberkey<L20n.FTL.AST.StringPrimitive>("keyword");
			checkValidMemberkey<L20n.FTL.AST.Attribute>("identifier/keyword");
			checkValidMemberkey<L20n.FTL.AST.Number>("42");

			// some invalid examples
			Throws(() => Memberkey.Parse(NCS("@#")));

			// for more examples check the individual types (keyword, identifier and number)
		}

		private void checkValidMember(string input, string rest = "")
		{
			var cs = NCS(input);
			var member = Member.Parse(cs);
			Assert.IsNotNull(member);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(L20n.FTL.AST.Member), member.GetType());
		}

		[Test(), Timeout(2000)]
		public void MemberTests()
		{
			checkValidMember("[keyword] something");
			checkValidMember("[0] no items");
			checkValidMember("[0] \"still no items\"");
			checkValidMember("[ui/desc] Hello, { $username }!");
			checkValidMember("[other] You have now { $count } items!");
		}
		
		private void checkValidMessage(string input, string rest = "")
		{
			var cs = NCS(input);
			var ctx = new Context(Context.ASTTypes.Full);
			L20n.FTL.AST.INode node;
			Assert.IsTrue(Message.PeekAndParse(cs, ctx, out node));
			Assert.IsNotNull(node);
			Assert.AreEqual(rest, cs.ReadUntilEnd());
			Assert.AreEqual(typeof(L20n.FTL.AST.Entity), node.GetType());
		}

		[Test(), Timeout(2000)]
		public void MessageTests()
		{
			checkValidMessage("hello = Hello, World!");
			checkValidMessage("hello = Hello, { $user }!");
			checkValidMessage("hello = \"Hello, { $user }!\"");
			checkValidMessage(@"brandName = Firefox
 									[gender] masculine");
			checkValidMessage(@"opened-new-window = { brandName[gender] ->
								 *[masculine] { brandName } otworzyl nowe okno.
								  [feminine] { brandName } otworzyla nowe okno.
								}");

			L20n.FTL.AST.INode node;
			var ctx = new Context(Context.ASTTypes.Full);
			Assert.IsFalse(Message.PeekAndParse(NCS("   error = space before message is not allowed"), ctx, out node));
			Assert.IsFalse(Message.PeekAndParse(NCS("\nerror = newline before message is not allowed"), ctx, out node));
		}

		[Test(), Timeout(2000)]
		public void ValidFTLFullBodyTest()
		{
			var streamReader = StreamReaderFactory.Create("../../resources/parser/example.ftl");
			var ctx = new Context(Context.ASTTypes.Full);
			var parser = new Parser();
			Assert.IsNotNull(parser.Parse(streamReader, ctx));
		}
		
		[Test(), Timeout(2000)]
		public void ValidFTLPartialBodyTest()
		{
			var streamReader = StreamReaderFactory.Create("../../resources/parser/example.ftl");
			var ctx = new Context(Context.ASTTypes.Partial);
			var parser = new Parser();
			Assert.IsNotNull(parser.Parse(streamReader, ctx));
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