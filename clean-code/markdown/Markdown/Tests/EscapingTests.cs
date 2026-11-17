using NUnit.Framework;
using FluentAssertions;

namespace Markdown.Tests
{
	[TestFixture]
	public class EscapingTests
	{
		private Md _md = new();

		[Test]
		public void EscapedUnderscoresNotParsed()
		{
			var rendered = _md.Render("\\_Вот это\\_");
			
			rendered.Should().Be("<p>_Вот это_</p>");
		}

		[Test]
		public void EscapedBackslashStays()
		{
			var rendered = _md.Render("сим\\\\волы");
			
			rendered.Should().Be("<p>сим\\волы</p>");
		}

		[Test]
		public void LoneBackslashStays()
		{
			var rendered = _md.Render("сим\\волы");
			
			rendered.Should().Be("<p>сим\\волы</p>");
		}

		[Test]
		public void DoubleEscapedUnderscoreBecomesEm()
		{
			var rendered = _md.Render("\\\\_вот это\\\\_");
			
			rendered.Should().Be("<p>\\<em>вот это\\</em></p>");
		}

		[Test]
		public void EscapedHashDoesNotCreateHeading()
		{
			var rendered = _md.Render("\\# Не заголовок");

			rendered.Should().Be("<p># Не заголовок</p>");
		}

		[Test]
		public void EscapedDashDoesNotCreateList()
		{
			var rendered = _md.Render("\\- Не список");

			rendered.Should().Be("<p>- Не список</p>");
		}
	}
}
