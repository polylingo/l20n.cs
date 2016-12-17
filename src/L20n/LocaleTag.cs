// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20n.Exceptions;
using System.Text.RegularExpressions;

namespace L20n
{
	/// <summary>
	/// A Locale Tag identifies a locale.
	/// It identifies the language, and optionally, its region and SIL/dialect.
	/// </summary>
	/// <description>
	/// More information at
	/// https://github.com/polylingo/docs/blob/master/l20n/locales.md
	/// </description>
	public sealed class LocaleTag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="L20n.LocaleTag"/> class.
		/// The given tag has to be a valid tag, or an InputException gets thrown.
		/// </summary>
		public LocaleTag(string tag)
		{
			string[] parts = tag.Split('-');
			if(parts.Length > 3) {
				throw new InputException(
					"`{0}` is an invalid tag, contains more then 3 parts", tag);
			}

			// set Language
			if(Regex.IsMatch(parts[0], @"^[a-zA-Z]+$")) {
				m_Language = parts[0].ToUpper();
			} else {
				throw new InputException(
					"`{0}` is an invalid language, contains non-alphabetic characters", parts[0]);
			}

			// set Region if possible
			if(parts.Length > 1) {
				if(Regex.IsMatch(parts[1], @"^[a-zA-Z]+$")) {
					m_Region = parts[1].ToUpper();
				} else {
					throw new InputException(
						"`{0}` is an invalid region, contains non-alphabetic characters", parts[1]);
				}

				// set Dialect/SIL if possible
				if(parts.Length == 3) {
					if(Regex.IsMatch(parts[2], @"^[a-zA-Z0-9]+$")) {
						m_Extension = parts[2].ToUpper();
					} else {
						throw new InputException(
							"`{0}` is an invalid dialect/SIL, contains non-alphanumeric characters", parts[2]);
					}
				}
			}
		}

		/// <summary>
		/// Compares the current Locale against the requested tag,
		/// and sees how well it fits the bill.
		/// A high value means it fits well, a low value means it doesn't.
		/// </summary>
		public int Compare(LocaleTag requested)
		{
			// if the language doesn't match, than a match is not possible
			if(m_Language != requested.m_Language)
				return 0;

			// if the region doesn't match,
			// we give a higher value if this localeTag is a pure-language tag.
			if(m_Region != requested.m_Region) {
				if(m_Region == null)
					return 3;

				return m_Extension == null ? 2 : 1;
			}

			// if the extension doesn't match,
			// we give a higher value if this localeTag is a pure-languageRegion tag.
			if(m_Extension != requested.m_Extension) {
				if(m_Extension == null)
					return 5;
				return 4;
			}

			// a perfect match,
			// this is exactly what we're looking for
			return 6;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="L20n.LocaleTag"/>.
		/// </summary>
		public override string ToString()
		{
			if(m_Region == null)
				return m_Language;

			if(m_Extension == null)
				return string.Format("{0}-{1}", m_Language, m_Region);

			return string.Format("{0}-{1}-{2}", m_Language, m_Region, m_Extension);
		}

		private readonly string m_Language;
		private readonly string m_Region;
		private readonly string m_Extension; // dialect/SIL
	}
}

