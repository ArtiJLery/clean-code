using NUnit.Framework;
using FluentAssertions;

namespace Markdown.Tests
{
	[TestFixture]
	public class StrongTests
	{
		private Md _md = new();

		[Test]
		public void SimpleStrong()
		{
			var rendered = _md.Render("Выделенный __текст__");
			
			rendered.Should().Be("<p>Выделенный <strong>текст</strong></p>");
		}

		[Test]
		public void StrongContainsEm()
		{
			var rendered = _md.Render("__с _разными_ символами__");
			
			rendered.Should().Be("<p><strong>с <em>разными</em> символами</strong></p>");
		}

		[Test]
		public void EmptyUnderscoresStay()
		{
			var rendered = _md.Render("____");
			
			rendered.Should().Be("<p>____</p>");
		}
	}
}
