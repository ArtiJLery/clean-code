using NUnit.Framework;
using FluentAssertions;

namespace Markdown.Tests
{
	[TestFixture]
	public class EmphasisTests
	{
		private Md _md = new();

		[Test]
		public void SimpleEm()
		{
			var rendered = _md.Render("Текст, _окруженный_ тегом.");
			
			rendered.Should().Be("<p>Текст, <em>окруженный</em> тегом.</p>");
		}

		[Test]
		public void EmphasisWorksWithinSingleNewline()
		{
			var rendered = _md.Render("_текст\nещё_");
			
			rendered.Should().Be("<p><em>текст ещё</em></p>");
		}

		[TestCase("сер_еди_не", "<p>сер<em>еди</em>не</p>")]
		[TestCase("_нач_але", "<p><em>нач</em>але</p>")]
		[TestCase("кон_це_", "<p>кон<em>це</em></p>")]
		public void EmInPartOfWord(string input, string expected)
		{
			var rendered = _md.Render(input);

			rendered.Should().Be(expected);
		}

		[Test]
		public void EmBetweenWordsNotWorking()
		{
			var rendered = _md.Render("ра_зных сл_овах");
			
			rendered.Should().Be("<p>ра_зных сл_овах</p>");
		}

		[Test]
		public void EmInsideDigitsNotWorking()
		{
			var rendered = _md.Render("цифры_12_3");
			
			rendered.Should().Be("<p>цифры_12_3</p>");
		}

		[Test]
		public void EmUnpairedStaysAsIs()
		{
			var rendered = _md.Render("_Непарные символы");
			
			rendered.Should().Be("<p>_Непарные символы</p>");
		}

		[Test]
		public void EmOpeningFollowedBySpaceStays()
		{
			var rendered = _md.Render("это_ подчерки_");
			
			rendered.Should().Be("<p>это_ подчерки_</p>");
		}

		[Test]
		public void EmClosingPrecededBySpaceStays()
		{
			var rendered = _md.Render("это _подчерки");
			
			rendered.Should().Be("<p>это _подчерки</p>");
		}
	}
}
