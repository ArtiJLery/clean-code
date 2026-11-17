using Markdown.Models;

namespace Markdown.Parsing
{
	public class EmphasisRule
	{
		// Правило для '_'.
		// Учитывает:
		//  - запрет выделения внутри цифр (цифры_12_3),
		//  - запрет через пробел с обеих сторон,
		//  - части слова (нач_але, сер_еди_не, кон_це) — работают,
		//  - через слова (ра_зных сл_овах) — нет,
		//  - внутри __ … __ работает, внутри _ … _ — нет.
		public bool TryMatch(RuleContext context, int pos, int end,
			out int emEnd, out int emLen)
		{
			emEnd = -1;
			emLen = 1;

			if (!IsStartWithCorrectChar(context, pos, end)) return false;
			if (IsPartOfDoubleUnderscore(context, pos, end)) return false;
			if (char.IsWhiteSpace(context.Text[pos + 1])) return false;

			var openInsideWord = IsInsideWord(context, pos);
			var i = pos + 1;
			var doubleUnderscoresCount = 0;
			while (i < end)
			{
				if (openInsideWord && char.IsWhiteSpace(context.Text[i])) return false;

				if (IsUnmaskedUnderscore(context, i))
				{
					// Проверяем, не является ли это подчеркивание частью двойного __
					var prevIsUnderscore = i - 1 >= pos && IsUnmaskedUnderscore(context, i - 1);
					var nextIsUnderscore = i + 1 < end && IsUnmaskedUnderscore(context, i + 1);

					// Если это часть двойного __, и при этом это не трёшка
					if (prevIsUnderscore || nextIsUnderscore)
					{
						doubleUnderscoresCount++;
						// Если это тройное ___ в конце, то последнее подчеркивание 
						// (где nextIsUnderscore == false, а prevIsUnderscore == true) 
						// должно сработать как закрывающее.
						// Если это просто __, мы его перепрыгиваем.
						if (nextIsUnderscore)
						{
							i += 2;
							continue;
						}
					}

					// Проверяем, что перед закрывающей не пробел
					if (IsValidClosingCandidate(context, i, pos))
					{
						if (IsBoundaryDigits(context, pos, i)) return false;

						// Если doubleUnderscoresCount нечётный, значит внути em
						// есть __ без пары, тогда появляется пересечение.
						if (doubleUnderscoresCount % 2 != 0) return false;

						emEnd = i;
						return true;
					}
				}
				i++;
			}
			return false;
		}

		private bool IsStartWithCorrectChar(RuleContext context, int pos, int end)
		{
			return pos + 1 < end &&
				   context.Text[pos] == '_' &&
				   !context.IsMasked(pos);
		}

		private bool IsPartOfDoubleUnderscore(RuleContext context, int pos, int end)
		{
			var prevIsUnderscore = pos > 0 && context.Text[pos - 1] == '_' && !context.IsMasked(pos - 1);
			var nextIsUnderscore = pos + 1 < end && context.Text[pos + 1] == '_' && !context.IsMasked(pos + 1);
			return prevIsUnderscore || nextIsUnderscore;
		}

		private bool IsInsideWord(RuleContext context, int pos)
		{
			return pos > 0 && char.IsLetterOrDigit(context.Text[pos - 1]);
		}

		private bool IsUnmaskedUnderscore(RuleContext context, int index)
		{
			return context.Text[index] == '_' && !context.IsMasked(index);
		}

		private bool IsValidClosingCandidate(RuleContext context, int i, int pos)
		{
			return i > pos + 1 && !char.IsWhiteSpace(context.Text[i - 1]);
		}

		private bool IsBoundaryDigits(RuleContext context, int pos, int i)
		{
			var innerLeftDigit = char.IsDigit(context.Text[pos + 1]);
			var innerRightDigit = char.IsDigit(context.Text[i - 1]);
			return innerLeftDigit && innerRightDigit;
		}
	}
}
