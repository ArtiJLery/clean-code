using NUnit.Framework;
using FluentAssertions;

namespace Markdown.Tests
{
	[TestFixture]
	public class HeadingTests
	{
		private Md _md = new();

		[Test]
		public void Simple_Heading()
		{
			var rendered = _md.Render("# Заголовок");
			
			rendered.Should().Be("<h1>Заголовок</h1>");
		}

		[Test]
		public void HeadingWithoutSpaceIsText()
		{
			var rendered = _md.Render("#Заголовок");

			rendered.Should().Be("<p>#Заголовок</p>");
		}
	}
}
