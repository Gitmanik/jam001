using System.Runtime.ConstrainedExecution;

namespace GitBrowser
{
	public class TreeBuilder
	{
		int ctr = 0;
		Queue<Token> Tokens;
		Token Peek() => Tokens.Peek();
		Token Lens(int v) => Tokens.ToArray()[v];
		Token Get() { ctr++; return Tokens.Dequeue(); }

		public TreeBuilder(List<Token> tokens) { Tokens = new(tokens); }

		public DOMElement BuildTree()
		{
			if (Tokens == null)
				throw new ArgumentNullException("Tokens");

			DOMElement root = new() { Name = "HTML", Value = null };

			root.Children.Add(BuildBranch());

			Console.WriteLine("BUILDTRE DONE");
			Console.WriteLine(root.PrettyPrint());

			return root;
		}

		private DOMElement BuildBranch()
		{
			Token root = Get();
			DOMElement el = new DOMElement() { Name = root.Data, Value = null, Attributes = root.Attributes };

			while (Tokens.Count > 0)
			{
				if (Peek().Type == Token.TokenType.COMMENT)
					continue;

				if (Peek().Type == Token.TokenType.TAG)
				{
					if (Peek().Data == root.Data && Peek().ClosingTag)
					{
						Get();
						break;
					}
					el.Children.Add(BuildBranch());
				}

			}

			return el;
		}
	}
}
