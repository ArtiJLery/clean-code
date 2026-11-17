using FluentAssertions;
using NUnit.Framework;

namespace Markdown.Tests
{
	[TestFixture]
	public class SpecExamplesTests
	{
		private Md _md = new();

		[Test]
		public void HeadingWithDifferentMarks_AndEmInStrong()
		{
			var rendered = _md.Render("# Заголовок __с _разными_ символами__");
			
			rendered.Should().Be("<h1>Заголовок <strong>с <em>разными</em> символами</strong></h1>");
		}

		[Test]
		public void HeadingWithStrong()
		{
			var rendered = _md.Render("# Заголовок с __разными__ символами");
			
			rendered.Should().Be("<h1>Заголовок с <strong>разными</strong> символами</h1>");
		}

		[Test]
		public void IntersectionOfEmAndStrong()
		{
			var rendered = _md.Render("_текст __не_ сильный__");
			
			rendered.Should().Be("<p>_текст __не_ сильный__</p>");
		}
	}
}
