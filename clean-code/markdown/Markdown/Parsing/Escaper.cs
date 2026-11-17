using System.Text;

namespace Markdown.Parsing
{
	public class Escaper
	{
		// Обрабатывает экранирование '\':
		// - '\_' → '_' (символ становится литералом),
		// - '\\' → '\',
		// - одиночный '\' перед обычным символом остаётся '\'.
		// Выполняется ДО разбора разметки.
		public const char Mask = '\u0001';
		private HashSet<char> _escapableChars;

		public Escaper(HashSet<char> escapableChars)
		{
			// теперь список символов для экранирования проиходит из Md
			// эскейпер отрабатывает первым, поэтому другие правила не могут сами сообщить ему свои символы
			_escapableChars = new HashSet<char>(escapableChars) { '\\', Mask };
		}

		public string Apply(string text)
		{
			// Возвращает "маскированную" строку, где экранированные символы
			// уже заменены на плейсхолдеры/литералы
			var sb = new StringBuilder(text.Length);
			var i = 0;
			while (i < text.Length)
			{
				var curChar = text[i];
				if (curChar == '\\' &&  i + 1 < text.Length)
				{
					var next = text[i + 1];
                    if (_escapableChars.Contains(next))
                    {
						sb.Append(Mask);
						sb.Append(next);
						i += 2;
						continue;
                    }
                }
				sb.Append(curChar);
				i++;
			}
			return sb.ToString();
		}
	}
}
