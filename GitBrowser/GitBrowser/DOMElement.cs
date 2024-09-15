namespace GitBrowser
{
	public class DOMElement
	{
		public required string Name;
		public Dictionary<string, string> Attributes = [];
		public List<DOMElement> Children = [];

		public string PrettyPrint(int indent = 0)
		{
			string Ind(int indent) { string ind = ""; for (int i = 0; i < indent; i++) ind += "\t"; return ind; }

			string ind = Ind(indent);

			string pr = $"{ind}[DOMElement: {Name}]\n" +
						$"{ind}Attributes:{string.Join(", ", Attributes.ToList().ConvertAll(x => $"{x.Key}:{x.Value}"))}\n" +
						$"{ind}Children:[\n";

			foreach (var child in Children)
				pr += $"{child.PrettyPrint(indent + 1)}\n";
			pr += $"{ind}]";
			return pr;
		}
		public override string ToString() => PrettyPrint();
	}
}