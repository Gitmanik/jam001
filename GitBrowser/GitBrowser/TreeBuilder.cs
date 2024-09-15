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
			Console.WriteLine("BuildTree");
			if (Tokens == null)
				throw new ArgumentNullException("Tokens");

			DOMElement root = new() { Name = "HTML" };

			try
			{
				root.Children.Add(BuildBranch());
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				Console.WriteLine(root);
				//Console.WriteLine($"Next Tokens:\n{string.Join("", Tokens.ToList())}");
			}

			Console.WriteLine("BuildTree DONE");
			Console.WriteLine(root.PrettyPrint());

			return root;
		}

		private DOMElement BuildBranch(int ctr = 0)
		{
			Token root = Get();
			DOMElement el = new DOMElement() { Name = root.Data, Attributes = root.Attributes };
			Console.WriteLine($"BuildBranch {ctr}: {el}");

			while (Tokens.Count > 0)
			{
				if (Peek().Type == Token.TokenType.COMMENT)
				{
					Get();
					continue;
				}

				if (Peek().Type == Token.TokenType.TAG)
				{
					if (Peek().Data == root.Data && Peek().ClosingTag)
					{
						Console.WriteLine($"Closing {el.Name}");
						Get();
						break;
					}
					if (Peek().ClosingTag)
					{
						Console.WriteLine($"WARN: {Peek().Data} is closing but root is {root.Data}");
						Get();
						break;
					}
					el.Children.Add(BuildBranch(ctr+1));
					continue;
				}

				if (Peek().Type == Token.TokenType.LITERAL)
				{
					el.Children.Add(new DOMElement { Name = "RAW_TEXT", Attributes = new Dictionary<string, string>() { ["value"] = Get().Data } });
				}

			}

			return el;
		}
	}
}
