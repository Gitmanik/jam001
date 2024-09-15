
using System.Reflection.Metadata.Ecma335;

namespace GitBrowser
{
	public class DOMElement
	{
		public required string Name;
		public required string? Value;
		public Dictionary<string, string> Attributes = [];
		public List<DOMElement> Children = [];

		public string PrettyPrint(int indent = 0)
		{
			string Ind(int indent) { string ind = ""; for (int i = 0; i < indent; i++) ind += "\t"; return ind; }
			string pr = $"{Ind(indent)} [DOMElement: {Name}]\n";

			return pr;
		}
	}
}