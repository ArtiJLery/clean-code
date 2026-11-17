using FluentAssertions;
using NUnit.Framework;
using System.Diagnostics;
using System.Text;

namespace Markdown.Tests
{
	[TestFixture]
	public class PerformanceTests
	{
		private const int WarmupSize = 1000;

		[Test]
		public void RenderLargeInputIsLinear()
		{
			var md = new Md();
			var chunk = "текст _курсив_ и __сильный__ # ещё - пункт ";

			// 1. Прогрев (JIT-компиляция)
			MeasureRenderTime(md, BuildInput(chunk, WarmupSize));

			// 2. Замеры для разных размеров (O(n) — время должно расти пропорционально)
			var time100k = MeasureRenderTime(md, BuildInput(chunk, 100_000));
			var time200k = MeasureRenderTime(md, BuildInput(chunk, 200_000));
			var time400k = MeasureRenderTime(md, BuildInput(chunk, 400_000));

			// Защита от деления на 0 (если вдруг отработало слишком быстро)
			if (time100k == 0) time100k = 1;

			// Проверяем, что при увеличении входа в 2 раза время растет не более чем в 3 раза.
			// Если бы алгоритм был O(n²), время выросло бы в 4 раза и более.
			var ratio1 = (double)time200k / time100k;
			var ratio2 = (double)time400k / time200k;

			ratio1.Should().BeLessThan(3.0,
				$"Время для 200k ({time200k}мс) выросло более чем в 3 раза по сравнению со 100k ({time100k}мс). Возможен O(n²).");
			ratio2.Should().BeLessThan(3.0,
				$"Время для 400k ({time400k}мс) выросло более чем в 3 раза по сравнению с 200k ({time200k}мс). Возможен O(n²).");
		}

		[Test]
		public void RenderManyUnderscoresIsLinear()
		{
			var md = new Md();

			// Генерируем строку вида: "___...___x___...___"
			string BuildUnderscores(int count) => new string('_', count / 2) + "x" + new string('_', count / 2);

			// Прогрев
			MeasureRenderTime(md, BuildUnderscores(WarmupSize));

			var time50k = MeasureRenderTime(md, BuildUnderscores(50_000));
			var time100k = MeasureRenderTime(md, BuildUnderscores(100_000));

			if (time50k == 0) time50k = 1;
			var ratio = (double)time100k / time50k;

			ratio.Should().BeLessThan(3.0,
				$"Время для 100k подчеркиваний ({time100k}мс) выросло более чем в 3 раза по сравнению с 50k ({time50k}мс). Возможен O(n²).");
		}

		private long MeasureRenderTime(Md md, string input)
		{
			var sw = Stopwatch.StartNew();
			md.Render(input);
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		private string BuildInput(string chunk, int targetLength)
		{
			var sb = new StringBuilder(targetLength + chunk.Length);
			while (sb.Length < targetLength)
			{
				sb.Append(chunk);
			}
			return sb.ToString();
		}
	}
}