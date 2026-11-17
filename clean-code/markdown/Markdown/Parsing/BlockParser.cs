using Markdown.Models;

namespace Markdown.Parsing
{
	// Теперь просто бьёт текст на абзацы и скидывает их AbzatsParser
	public class BlockParser
	{
		private AbzatsParser _abzatsParser;
		private AbzatsSplitter _splitter;

		public BlockParser(AbzatsParser abzatsParser)
		{
			_abzatsParser = abzatsParser;
			_splitter = new AbzatsSplitter();
		}

		public IReadOnlyList<INode> Parse(string text)
		{
			var abzatsList = _splitter.Split(text);
			var blocks = new List<INode>();

			foreach (var abzats in abzatsList)
			{
				blocks.Add(_abzatsParser.Parse(abzats));
			}

			return blocks;
		}
	}
}
