namespace Markdown.Parsing
{
	public class AbzatsSplitter
	{
		private const char LineFeed = '\n';
		private const char Space = ' ';
		private const char Tab = '\t';

		// Делит исходный текст на абзацы. Абзац — единица обработки правил
		// Два и более \n это новый абзац, один - просто новая строка
		public IReadOnlyList<string> Split(string text)
		{
			// Привязка к Environment.NewLine может сделать код специализированным для Windows
			// Нормализация позволит обрабатывать строки не зависимо от ОС
			var normalizedText = text.Replace("\r\n", "\n").Replace("\r", "\n");
			var result = new List<string>();
			var start = 0;
			var i = 0;

			while (i < normalizedText.Length)
			{
				if (normalizedText[i] == LineFeed)
				{
					var j = i + 1;
					// Пропускаем горизонтальные пробелы
					while (j < normalizedText.Length && IsHorizontalWhitespace(normalizedText[j]))
					{
						j++;
					}
					if (j < normalizedText.Length && normalizedText[j] == LineFeed)
					{
						// Следующий абзац сразу после второго \n
						// Остальные \n останутся в начале следующего абзаца
						AddIfNonEmpty(result, normalizedText, start, i);
						start = j + 1;
						i = start;
						continue;
					}
					else
					{
						// Одиночный \n, не граница
						i = j;
						continue;
					}
				}
				i++;
			}
			AddIfNonEmpty(result, normalizedText, start, normalizedText.Length);
			return result;
		}

		private static bool IsHorizontalWhitespace(char ch)
		{
			return ch == Space || ch == Tab;
		}
			
		private void AddIfNonEmpty(List<string> result, string text, int start, int end)
		{
			if (end > start)
			{
				result.Add(text.Substring(start, end - start));
			}
		}
	}
}
