namespace GitBrowser;

public class Token
{
	public enum TokenType
	{
		TAG,
		LITERAL,
		COMMENT,
	}

	public Dictionary<string, string> Attributes = new();

	public bool ClosingTag = false;

	public required string Data;
	public required TokenType Type;

	public override string ToString()
	{
		return $"TOKEN: {Type} [{Data}], Closing:{ClosingTag},{string.Join(", ", Attributes.ToList().ConvertAll(x => $"{x.Key}:{x.Value}"))}";
	}
}

public class Tokenizer
{
	public Tokenizer(string html)
	{
		dataQueue = new(html.ToCharArray());
	}

	private readonly char[] WhitespaceChars = new char[] { ' ', '\n', '\r', '\t' };
	private readonly char[] QuoteChars = new char[] { '\'', '"', '`' };

	Queue<char> dataQueue;
	int ctr = 0;
	char Peek() => dataQueue.Peek();
	char Lens(int v) => dataQueue.ToArray()[v];
	char Get() { ctr++; return dataQueue.Dequeue(); }

	public List<Token> Tokenize()
	{
		var tokens = new List<Token>();

		while (dataQueue.Count > 0)
		{
			try
			{
				if (Peek() == '<')
				{
					tokens.Add(ParseTag());
					continue;
				}

				if (WhitespaceChars.Contains(Peek()))
				{
					Get();
					continue;
				}
				Console.WriteLine($"T: {Peek()}");
				tokens.Add(ParseLiteral());
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				Console.WriteLine($"Next 25 chars from HTML:\n{string.Join("", dataQueue.ToArray()).Substring(0, Math.Min(dataQueue.ToArray().Length, 25))}");
				break;
			}
		}
		Console.WriteLine("DONE");
		Console.WriteLine(string.Join("\n", tokens));

		return tokens;
	}

	private Token ParseLiteral()
	{
		string data = "";
		while (dataQueue.Count > 0)
		{
			if (Peek() == '<')
				break;
			data += Get();
		}
		return new Token() { Type = Token.TokenType.LITERAL, Data = data };
	}

	private Token ParseTag()
	{
		Get();
		string data = "";
		var tk = new Token() { Type = Token.TokenType.TAG, Data = "" };

		while (dataQueue.Count > 0)
		{
			Console.WriteLine($"PT: {Peek()}, {data}");
			if (Peek() == '>')
			{
				Get();
				break;
			}

			if (WhitespaceChars.Contains(Peek()))
			{
				while (WhitespaceChars.Contains(Peek()))
					Get();
				ParseAttribute(tk);
				continue;
			}

			if (Peek() == '/')
			{
				tk.ClosingTag = true;
				Get();
				continue;
			}

			if (Peek() == '!')
				return ParseComment();


			if (!char.IsAsciiLetterOrDigit(Peek()))
				throw new TokenizerException($"Non-letter at idx {ctr}! [{Peek()}]");

			data += Get();
		}

		tk.Data = data;
		return tk;
	}

	private void ParseAttribute(Token tk)
	{
		bool readData = false;

		string title = "";
		string data = "";

		char? quoteChar = null;

		while (dataQueue.Count > 0)
		{
			Console.WriteLine($"PA: {Peek()}, {quoteChar}, {readData}");
			if (quoteChar == null && WhitespaceChars.Contains(Peek()))
				return;

			if (quoteChar == null && QuoteChars.Contains(Peek()))
			{
				quoteChar = Get();
				Console.WriteLine($"QuoteChar: {quoteChar}");
				continue;
			}
			if (quoteChar != null && Peek() == quoteChar)
			{
				quoteChar = null;
				Get();
				continue;
			}

			if (quoteChar != null && Peek() != quoteChar)
			{
				goto forceRead;
			}

			if (Peek() == '>')
			{
				break;
			}

			if (Peek() == '=')
			{
				readData = true;
				Get();
				continue;
			}

			if (quoteChar == null && !char.IsAsciiLetterOrDigit(Peek()))
				throw new TokenizerException($"Wrong character at idx {ctr}! Expected letter or digit, got [{Peek()}]");

			forceRead:
			if (readData)
				data += Get();
			else
				title += Get();
		}
		tk.Attributes[title] = data;
	}

	private Token ParseComment()
	{
		string data = "";
		Get(); //!
		Get(); //-
		Get(); //-
			   //TODO: CHECK
		while (dataQueue.Count > 0)
		{
			if (Peek() == '-' && Lens(1) == '-' && Lens(2) == '>')
			{
				Get();
				Get();
				Get();
				break;
			}
			data += Get();
		}
		return new Token() { Type = Token.TokenType.COMMENT, Data = data };
	}
}

[Serializable]
internal class TokenizerException : Exception
{
	public TokenizerException()
	{
	}

	public TokenizerException(string? message) : base(message)
	{
	}

	public TokenizerException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
