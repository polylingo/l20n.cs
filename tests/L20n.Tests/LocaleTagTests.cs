// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

using L20n;
using L20n.Exceptions;

namespace L20nTests
{
	[TestFixture()]
	public sealed class LocaleTagTests
	{
		[Test()]
		public void ValidLocaleTests()
		{
			// textbook pure language examples
			Assert.AreEqual("EN", new LocaleTag("en").ToString());
			Assert.AreEqual("NL", new LocaleTag("NL").ToString());

			// anti-idiomatic, but valid pure language examples
			Assert.AreEqual("FR", new LocaleTag("Fr").ToString());
			Assert.AreEqual("ENGLISH", new LocaleTag("english").ToString());

			// textbook language-region examples
			Assert.AreEqual("EN-US", new LocaleTag("en-us").ToString());
			Assert.AreEqual("NL-BE", new LocaleTag("NL-be").ToString());
			
			// anti-idiomatic, but valid language-region examples
			Assert.AreEqual("NL-NL", new LocaleTag("NL-nl").ToString());
			Assert.AreEqual("EN-GB", new LocaleTag("en-GB").ToString());
			Assert.AreEqual("FOO-BAR", new LocaleTag("foo-bar").ToString());

			// full languageTag examples
			Assert.AreEqual("FOO-BAR-BAZ0123", new LocaleTag("foo-BaR-BAZ0123").ToString());
			Assert.AreEqual("ONE-ONE-2", new LocaleTag("one-one-2").ToString());
		}

		[Test()]
		public void InvalidLocaleTests()
		{
			// Empty parts are not allowed
			Throws(() => new LocaleTag(""));
			Throws(() => new LocaleTag("foo--baz"));
			Throws(() => new LocaleTag("-"));
			Throws(() => new LocaleTag("--"));
			Throws(() => new LocaleTag("-bar-"));

			// Using wrong separator
			Throws(() => new LocaleTag("en_US"));
			Throws(() => new LocaleTag("en-GB_foo"));
			Throws(() => new LocaleTag("en+GB"));

			// Having more then 3 parts
			Throws(() => new LocaleTag("one-two-three-four"));
		}

		private void CompareTest(string rawRequested, string[] rawInput, string[] expected)
		{
			LocaleTag[] input = new LocaleTag[rawInput.Length];
			for(int i = 0; i < rawInput.Length; ++i)
				input[i] = new LocaleTag(rawInput[i]);

			LocaleTag requested = new LocaleTag(rawRequested);

			Array.Sort(input, (x, y) => {
				int rx = x.Compare(requested);
				int ry = y.Compare(requested);
				return ry - rx;
			});

			for(int i = 0; i < input.Length; ++i) {
				Assert.AreEqual(expected[i], input[i].ToString(),
				                "expected #{0} to be {1}, but was {2} instead",
				                i, expected[i], input[i].ToString());
			}
		}
		
		[Test()]
		public void CompareTests()
		{
			CompareTest("EN",
			            new string[]{"EN-GB", "EN", "NL"},
						new string[]{"EN", "EN-GB", "NL"});
			CompareTest("EN-US",
			            new string[]{"FR", "EN", "EN-US-FOO", "EN-GB", "EN-US"},
			            new string[]{"EN-US", "EN-US-FOO", "EN", "EN-GB", "FR"});
			CompareTest("EN-US-FOO",
			            new string[]{"EN-GB-BAR", "EN-US-FOO", "EN", "EN-US", "EN-US-BAR", "EN-GB"},
						new string[]{"EN-US-FOO", "EN-US", "EN-US-BAR", "EN", "EN-GB", "EN-GB-BAR"});
		}
		
		private delegate void ThrowFunction();
		private static void Throws(ThrowFunction f)
		{
			try {
				f();
				Assert.IsFalse(true, "does not throw, while it was expected");
			} catch(InputException e) {
				Assert.IsNotNull(e);
			}
		}
	}
}

