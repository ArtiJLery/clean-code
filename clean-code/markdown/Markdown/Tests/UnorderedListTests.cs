using NUnit.Framework;
using FluentAssertions;

namespace Markdown.Tests
{
	[TestFixture]
	public class UnorderedListTests
	{
		private Md _md = new();

		[Test]
		public void SingleItemList()
		{
			var rendered = _md.Render("- один");
			
			rendered.Should().Be("<ul><li>один</li></ul>");
		}

		[Test]
		public void MultipleItemsList()
		{
			var rendered = _md.Render("- раз\n- два\n- три");
			
			rendered.Should().Be("<ul><li>раз</li><li>два</li><li>три</li></ul>");
		}

		[Test]
		public void ListItemWithFormatting()
		{
			var rendered = _md.Render("- _курсив_");
			
			rendered.Should().Be("<ul><li><em>курсив</em></li></ul>");
		}

		[Test]
		public void ListDoNotFollowedByParagraph()
		{
			var rendered = _md.Render("- раз\n\nтекст");
			
			rendered.Should().Be("<ul><li>раз</li></ul><p>текст</p>");
		}

		[Test]
		public void ListItemWithContinuationLine()
		{
			var rendered = _md.Render("- фыв\nэто тоже элемент 1 списка\n- 123");

			rendered.Should().Be("<ul><li>фыв это тоже элемент 1 списка</li><li>123</li></ul>");
		}

		[Test]
		public void ListItemWithContinuationAndFormatting()
		{
			var rendered = _md.Render("- первый _курсив_\nпродолжение __сильный__\n- второй");

			rendered.Should().Be("<ul><li>первый <em>курсив</em> продолжение <strong>сильный</strong></li><li>второй</li></ul>");
		}
	}
}
