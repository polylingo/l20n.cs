// // Glen De Cauwsemaecker licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

using L20n.IO;
using L20n.FTL.Parsers;
using L20n.Exceptions;

namespace L20nTests
{
	[TestFixture()]
	public sealed class BodyReflectionTests
	{
		void testValidReflection(string input, string expected = null)
		{
			var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			var streamReader = new StreamReader(memoryStream);

			var parser = new Parser();
			var body = parser.Parse(streamReader, new Context(Context.ASTTypes.Full));
			Assert.IsNotNull(body);

			Writer writer = new Writer(new StringWriter());
			body.Serialize(writer);

			if(expected == null)
				expected = input;

			Assert.AreEqual(expected, writer.ToString());
		}

		[Test(), Timeout(2000)]
		public void ValidReflectionTests()
		{
			testValidReflection("");
			testValidReflection("# a comment\n");
			testValidReflection("[[ some section ]]\n");
			testValidReflection("hello = Hello, World!\n");
			testValidReflection("hello = Hello, { $name }!\n");

			testValidReflection(@"# a simple hello world example
hello = Hello, World!
");

			testValidReflection(@"# a simple hello user example
hello = Hello, { $user }!
");

			testValidReflection(@"hello = Hello, { $name }!
  [extraInfo] whatever
foo = bar
");

			testValidReflection(@"hello = Hello, { $name }!
foo = bar
");

			testValidReflection(@"hello = Hello, { $name }!
[[ section ]]
brandName = whatever
# some kind of comment
animal = lion
");

			testValidReflection(@"screenWidth = 
  | The label of the button below changes depending on the
  | available screen width, allowing for a responsive design.
");
			
			testValidReflection("opened-new-window = { brandName[gender] ->\n" +
				"  *[masculine] { brandName } otworzyl nowe okno.\n" +
				"  [feminine] { brandName } otworzyla nowe okno.\n}\n",
	            "opened-new-window = { brandName[gender] ->\n" +
	            "  *[masculine] { brandName } otworzyl nowe okno.\n" +
	            "  [feminine] { brandName } otworzyla nowe okno. }\n");
		}
	}
}