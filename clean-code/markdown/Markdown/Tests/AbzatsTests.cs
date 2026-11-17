using NUnit.Framework;
using FluentAssertions;

namespace Markdown.Tests
{
	[TestFixture]
	public class AbzatsTests
	{
		private Md _md = new();

		[Test]
		public void SingleNewlineIsNotParagraphBreak()
		{
			// Одиночный перенос - пробел при рендере.
			var rendered = _md.Render("первая строка\nвторая строка");

			rendered.Should().Be("<p>первая строка вторая строка</p>");
		}

		[Test]
		public void DoubleNewlineIsParagraphBreak()
		{
			var rendered = _md.Render("Это первый абзац.\n\nЭто второй абзац.");

			rendered.Should().Be("<p>Это первый абзац.</p><p>Это второй абзац.</p>");
		}

		[Test]
		public void UnpairedEmphasisDoesNotMergeAcrossAbzats()
		{
			// _ из первого абзаца не должно закрыться _ из второго.
			var rendered = _md.Render("_текст\n\nещё_текст");
			
			rendered.Should().Be("<p>_текст</p><p>ещё_текст</p>");
		}

		[Test]
		public void ThreeNewlinesAreStillOneBreak()
		{
			var rendered = _md.Render("а\n\n\nb");
			
			rendered.Should().Be("<p>а</p><p> b</p>");
		}

		[Test]
		public void EmptyInputYieldsEmptyOutput()
		{
			var rendered = _md.Render("");

			rendered.Should().BeEmpty();
		}

		[Test]
		public void ParagraphPreservesLeadingSpaces()
		{
			// Пробелы в начале второго абзаца не должны съедаться
			var rendered = _md.Render("First\n  \n  Second");

			rendered.Should().Be("<p>First</p><p>  Second</p>");
		}

		[Test]
		public void WindowsLineEndingsAreHandled()
		{
			// Проверка работы с \r\n (Windows)
			var rendered = _md.Render("Первая строка\r\n\r\nВторая строка");

			rendered.Should().Be("<p>Первая строка</p><p>Вторая строка</p>");
		}

		[Test]
		public void MultipleNewlinesPreserveEmptySpace()
		{
			// В старом коде все \n схлопывались. Теперь лишние \n становятся пробелами
			// (так как одиночный \n внутри абзаца меняется на пробел).
			var rendered = _md.Render("а\n\n\nb");

			rendered.Should().Be("<p>а</p><p> b</p>");
		}
	}
}
