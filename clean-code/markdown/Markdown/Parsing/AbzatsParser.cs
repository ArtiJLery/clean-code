using Markdown.Models;
using System.Text;

namespace Markdown.Parsing
{
	// Разбирает один абзац (блок) на конкретный узел (заголовок, список или параграф)
	// Разбирает текст на блоки:
	// строка, начинающаяся с "# " - HeadingNode
	// строка, начинающаяся с "- " - элемент списка (собираются в UnorderedListNode).
	// Внутри абзаца одиночные '\n' пробел при рендере.
	// иначе ParagraphNode.
	public class AbzatsParser
	{
		private const string HeadingMarker = "# ";
		private const string ListItemMarker = "- ";

		private InlineParser _inline;

		public AbzatsParser(InlineParser inline)
		{
			_inline = inline;
		}

		public INode Parse(string abzats)
		{
			if (abzats.Length > 0 && abzats[0] == Escaper.Mask)
			{
				var paragraph = abzats.Replace('\n', ' ');
				return new ParagraphNode(_inline.Parse(paragraph));
			}
			if (abzats.StartsWith(HeadingMarker))
			{
				var content = abzats.Substring(HeadingMarker.Length).Replace('\n', ' ');
				return new HeadingNode(_inline.Parse(content));
			}

			// Если блок начинается с маркера списка — парсим как список
			if (abzats.StartsWith(ListItemMarker))
			{
				return ParseUnorderedList(abzats);
			}

			// Иначе это обычный параграф
			var paragraphText = abzats.Replace('\n', ' ');
			return new ParagraphNode(_inline.Parse(paragraphText));
		}

		private INode ParseUnorderedList(string abzats)
		{
			var items = new List<INode>();
			// Внутри одного абзаца - списка элементы разделены одинарным \n.
			// Поэтому сплит внутри парсера списка необходим, чтобы отделить элементы друг от друга.
			var lines = abzats.Split('\n');

			var currentItemBuilder = new StringBuilder();

			foreach (var line in lines)
			{
				if (line.StartsWith(ListItemMarker))
				{
					// Если это не первый элемент, сохраняем предыдущий
					if (currentItemBuilder.Length > 0)
					{
						items.Add(new ListItemNode(_inline.Parse(currentItemBuilder.ToString().Trim())));
						currentItemBuilder.Clear();
					}

					// Начинаем новый элемент, отрезая маркер
					currentItemBuilder.Append(line.Substring(ListItemMarker.Length));
				}
				else
				{
					// Без маркера продолжение текущего элемента списка
					// Добавляем пробел, чтобы текст не слипался
					currentItemBuilder.Append(' ').Append(line.Trim());
				}
			}

			// Не забываем добавить последний элемент
			if (currentItemBuilder.Length > 0)
			{
				items.Add(new ListItemNode(_inline.Parse(currentItemBuilder.ToString().Trim())));
			}

			return new UnorderedListNode(items);
		}
	}
}