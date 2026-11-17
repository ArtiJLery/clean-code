using Markdown.Models;

namespace Markdown.Parsing
{
	// Правило для '__'.
	// Взаимодействует с EmphasisRule: внутри strong одинарные работают,
	// при пересечении ___…___ ни один не считается выделением.
	// Пустое ____ (пустая строка) — литералы.
	public class StrongRule
	{
		public bool TryMatch(RuleContext context, int pos, int end,
			out int strongEnd, out int openLen)
		{
			strongEnd = -1;
			openLen = 2;

			if (!IsStartWithDoubleUnderscore(context, pos, end)) return false;
			if (char.IsWhiteSpace(context.Text[pos + 2])) return false;
			if (IsEmptyStrong(context, pos, end)) return false;

			var oneUnderscoresCount = 0;
			var i = pos + 2;
			while (i + 1 < end)
			{
				if (context.Text[i] == '_' && context.Text[i + 1] != '_') 
				{
					oneUnderscoresCount++;
				}
				if (i + 2 < end && context.Text[i + 2] == '_' && !context.IsMasked(i + 2))
				{
					i++;
					continue;
				}
				if (IsClosingDoubleUnderscore(context, i))
				{
					if (IsValidStrongClose(context, i, pos, oneUnderscoresCount))
					{
						strongEnd = i;
						return true;
					}
				}
				i++;
			}
			return false;
		}

		private bool IsStartWithDoubleUnderscore(RuleContext context, int pos, int end)
		{
			// Проверяем, что до конца строки хватает символов для открывающего __
			if (pos + 2 >= end) return false;
			if (context.Text[pos] != '_' || context.Text[pos + 1] != '_') return false;
			if (context.IsMasked(pos) || context.IsMasked(pos + 1)) return false;
			return true;
		}

		private bool IsEmptyStrong(RuleContext context, int pos, int end)
		{
			// Защита от пустого выделения (____ — это литералы)
			return pos + 3 < end && context.Text[pos + 2] == '_' && context.Text[pos + 3] == '_';
		}

		private bool IsClosingDoubleUnderscore(RuleContext context, int i)
		{
			return context.Text[i] == '_' && context.Text[i + 1] == '_'
				   && !context.IsMasked(i) && !context.IsMasked(i + 1);
		}

		private bool IsValidStrongClose(RuleContext context, int i, int pos, int oneUnderscoresCount)
		{
			// Перед закрывающей не должно быть пробела
			if (i <= pos + 2 || char.IsWhiteSpace(context.Text[i - 1])) return false;

			// Нечётное количество _ внутри пары __, значит есть пересечение
			if (oneUnderscoresCount % 2 != 0) return false;

			return true;
		}
	}
}
